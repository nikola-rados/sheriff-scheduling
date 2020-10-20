using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Db.models;
using SS.Db.models.sheriff;

namespace SS.Api.services
{
    /// <summary>
    /// Since the Sheriff is a derived class of User, this should handle the Sheriff side of things. 
    /// </summary>
    public class SheriffService
    {
        private ClaimsPrincipal User { get; }
        private readonly SheriffDbContext _db;
        public SheriffService(SheriffDbContext db, IHttpContextAccessor httpContextAccessor = null)
        {
            _db = db;
            User = httpContextAccessor?.HttpContext.User;
        }

        #region Sheriff

        public async Task<Sheriff> CreateSheriff(Sheriff sheriff)
        {
            await CheckForBlankOrDuplicateIdirName(sheriff.IdirName);
            await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            sheriff.IdirName = sheriff.IdirName.ToLower();
            sheriff.AwayLocation = null;
            sheriff.Training = null;
            sheriff.Leave = null;
            sheriff.HomeLocation = await _db.Location.FindAsync(sheriff.HomeLocationId);
            sheriff.IsEnabled = true;
            await _db.Sheriff.AddAsync(sheriff);
            await _db.SaveChangesAsync();
            return sheriff;
        }


        public async Task<List<Sheriff>> GetSheriffs()
        {
            var fiveDaysFromNow = DateTimeOffset.Now.AddDays(5).Date;
            var now = DateTimeOffset.Now.Date;

            var sheriffQuery = _db.Sheriff.AsNoTracking()
                .AsSplitQuery()

                //Apply permission filters.
                .ApplyPermissionFilters(User)
                //Include AwayLocation/Training/Leave that is within 5 days. 
                .Include(s => s.AwayLocation.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Training.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.Leave.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(l => l.LeaveType)
                .Include(s => s.HomeLocation)
                .Include(s => s.UserRoles)
                .ThenInclude(ur => ur.Role);

            var sheriffs = await sheriffQuery.ToListAsync();

            return sheriffs;
        }

        public async Task<Sheriff> GetSheriff(Guid id)
        {
            var today = DateTimeOffset.Now.Date;
            return await _db.Sheriff.AsNoTracking().AsSingleQuery()
                .ApplyPermissionFilters(User)
                .Include(s=> s.HomeLocation)
                .Include(s => s.AwayLocation.Where (al => al.EndDate >= today && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Leave.Where(l => l.EndDate >= today && l.ExpiryDate == null))
                .ThenInclude(l => l.LeaveType)
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sheriff> UpdateSheriff(Sheriff sheriff)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"Sheriff with the id: {sheriff.Id} could not be found. ");

            if (sheriff.BadgeNumber != savedSheriff.BadgeNumber)
                await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            _db.Entry(savedSheriff).CurrentValues.SetValues(sheriff);

            _db.Entry(savedSheriff).Property(x => x.HomeLocationId).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.IsEnabled).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.Photo).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.KeyCloakId).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.IdirId).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.IdirName).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.LastLogin).IsModified = false;

            await _db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<byte[]> GetPhoto(Guid id)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"No sheriff with Id: {id}");
            return savedSheriff.Photo;
        }

        public async Task<Sheriff> UpdateSheriffPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var savedSheriff = await _db.Sheriff.FirstOrDefaultAsync(s => (id.HasValue && s.Id == id) || (!id.HasValue && s.BadgeNumber == badgeNumber));
            savedSheriff.ThrowBusinessExceptionIfNull($"No sheriff with Badge: {badgeNumber} or Id: {id}");
            savedSheriff.Photo = photoData;
            await _db.SaveChangesAsync();
            return savedSheriff;
        }

        public async Task UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"Sheriff with the id: {id} could not be found. ");
            savedSheriff.HomeLocation = await _db.Location.FindAsync(locationId);
            savedSheriff.HomeLocation.ThrowBusinessExceptionIfNull($"Location with the id: {locationId} could not be found. ");
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(SheriffAwayLocation awayLocation)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);
            await ValidateNoOverlapAsync(awayLocation);

            awayLocation.Location = await _db.Location.FindAsync(awayLocation.LocationId);
            awayLocation.Sheriff = await _db.Sheriff.FindAsync(awayLocation.SheriffId);
            await _db.SheriffAwayLocation.AddAsync(awayLocation);
            await _db.SaveChangesAsync();
            return awayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(SheriffAwayLocation awayLocation)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);
            
            var savedAwayLocation = await _db.SheriffAwayLocation.FindAsync(awayLocation.Id);
            savedAwayLocation.ThrowBusinessExceptionIfNull($"{nameof(awayLocation)} with the id: {awayLocation.Id} could not be found. ");

            await ValidateNoOverlapAsync(awayLocation, awayLocation.Id);
            
            _db.Entry(savedAwayLocation).CurrentValues.SetValues(awayLocation);
            await _db.SaveChangesAsync();
            return awayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id)
        {
            var sheriffAwayLocation = await _db.SheriffAwayLocation.FindAsync(id);
            sheriffAwayLocation.ThrowBusinessExceptionIfNull(
                $"SheriffAwayLocation with the id: {id} could not be found. ");
            sheriffAwayLocation.ExpiryDate = DateTimeOffset.Now;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Leave

        public async Task<SheriffLeave> AddSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);
            await ValidateNoOverlapAsync(sheriffLeave);

            sheriffLeave.LeaveType = await _db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.Sheriff = await _db.Sheriff.FindAsync(sheriffLeave.SheriffId);
            await _db.SheriffLeave.AddAsync(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);

            var savedLeave = await _db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            await ValidateNoOverlapAsync(sheriffLeave, sheriffLeave.Id);

            _db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task RemoveSheriffLeave(int id)
        {
            var sheriffLeave = await _db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            sheriffLeave.ExpiryDate = DateTimeOffset.Now;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Training

        public async Task<SheriffTraining> AddSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);
            await ValidateNoOverlapAsync(sheriffTraining);

            sheriffTraining.Sheriff = await _db.Sheriff.FindAsync(sheriffTraining.SheriffId);
            sheriffTraining.TrainingType = await _db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await _db.SheriffTraining.AddAsync(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);
            
            var savedTraining = await _db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(savedTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            await ValidateNoOverlapAsync(sheriffTraining, sheriffTraining.Id);

            _db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task RemoveSheriffTraining(int id)
        {
            var sheriffTraining = await _db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffTraining)} with the id: {id} could not be found. ");
            sheriffTraining.ExpiryDate = DateTimeOffset.Now;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        #region Validation
        private async Task CheckForBlankOrDuplicateIdirName(string idirName)
        {
            if (string.IsNullOrEmpty(idirName))
                throw new BusinessLayerException($"Missing {nameof(idirName)}.");

            var existingSheriffWithIdir = await _db.Sheriff.FirstOrDefaultAsync(s => s.IdirName.ToLower() == idirName.ToLower());
            if (existingSheriffWithIdir != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithIdir.LastName}, {existingSheriffWithIdir.FirstName} has IDIR name: {existingSheriffWithIdir.IdirName}");
        }

        private async Task CheckForBlankOrDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                throw new BusinessLayerException($"Missing {nameof(badgeNumber)}.");

            var existingSheriffWithBadge = await _db.Sheriff.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
            if (existingSheriffWithBadge != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithBadge.LastName}, {existingSheriffWithBadge.FirstName} already has badge number: {badgeNumber}");
        }

        private void ValidateStartAndEndDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate >= endDate) throw new BusinessLayerException("The start datetime cannot be after or on the end datetime. ");
        }

        private async Task ValidateSheriffExists(Guid sheriffId)
        {
            if (!await _db.Sheriff.AsNoTracking().AnyAsync(s => s.Id == sheriffId))
                throw new BusinessLayerException($"Sheriff with id: {sheriffId} does not exist.");
        }

        private async Task ValidateNoOverlapAsync<T>(T data, int? updateOnlyId = null) where T : SheriffEvent
        {
            var entity = await _db.Set<T>().FirstOrDefaultAsync(sal =>
                sal.SheriffId == data.SheriffId && !(sal.StartDate > data.EndDate || data.StartDate > sal.EndDate) &&
                sal.ExpiryDate == null && 
                (!updateOnlyId.HasValue ||
                 updateOnlyId.HasValue && sal.Id != updateOnlyId));

            if (entity != null)
                throw new BusinessLayerException($"This overlaps with existing SheriffEvent {entity.Id} with date range: {entity.StartDate} to {entity.EndDate}");
        }
        
        #endregion

        #endregion
    }

}
