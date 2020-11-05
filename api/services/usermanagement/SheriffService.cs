using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Db.models;
using SS.Db.models.sheriff;

namespace SS.Api.services.usermanagement
{
    /// <summary>
    /// Since the Sheriff is a derived class of User, this should handle the Sheriff side of things. 
    /// </summary>
    public class SheriffService
    {
        private ClaimsPrincipal User { get; }
        private SheriffDbContext Db { get; }
        public SheriffService(SheriffDbContext db, IHttpContextAccessor httpContextAccessor = null)
        {
            Db = db;
            User = httpContextAccessor?.HttpContext.User;
        }

        #region Sheriff

        public async Task<Sheriff> AddSheriff(Sheriff sheriff)
        {
            await CheckForBlankOrDuplicateIdirName(sheriff.IdirName);
            await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            sheriff.IdirName = sheriff.IdirName.ToLower();
            sheriff.AwayLocation = null;
            sheriff.Training = null;
            sheriff.Leave = null;
            sheriff.HomeLocation = await Db.Location.FindAsync(sheriff.HomeLocationId);
            sheriff.IsEnabled = true;
            await Db.Sheriff.AddAsync(sheriff);
            await Db.SaveChangesAsync();
            return sheriff;
        }

        //Used for the Shift scheduling screen.
        public async Task<List<Sheriff>> GetSheriffsForShiftAvailability(int locationId, DateTimeOffset start, DateTimeOffset end, Guid? sheriffId = null)
        {
            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .Where(s =>
                    (sheriffId == null || sheriffId != null && s.Id == sheriffId) && 
                    s.IsEnabled &&
                    s.HomeLocationId == locationId ||
                    s.AwayLocation.Any(al =>
                        al.LocationId == locationId && !(al.StartDate > end || start > al.EndDate)
                                                    && al.ExpiryDate == null))
                .IncludeSheriffEventsBetweenDates(start, end);

            return await sheriffQuery.ToListAsync();
        }

        public async Task<List<Sheriff>> GetSheriffsForTeams()
        {
            var today = DateTimeOffset.UtcNow;
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .ApplyPermissionFilters(User, today, sevenDaysFromNow)
                .IncludeSheriffEventsBetweenDates(today, sevenDaysFromNow);
         
            return await sheriffQuery.ToListAsync();
        }

        public async Task<Sheriff> GetSheriffForTeams(Guid id)
        {
            var today = DateTimeOffset.UtcNow;
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            return await Db.Sheriff.AsNoTracking().AsSingleQuery()
                .ApplyPermissionFilters(User, today, sevenDaysFromNow)
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
            var savedSheriff = await Db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {sheriff.Id} could not be found. ");

            if (sheriff.BadgeNumber != savedSheriff.BadgeNumber)
                await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            Db.Entry(savedSheriff).CurrentValues.SetValues(sheriff);

            Db.Entry(savedSheriff).Property(x => x.HomeLocationId).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.IsEnabled).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.Photo).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.LastPhotoUpdate).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.KeyCloakId).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.IdirId).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.IdirName).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.LastLogin).IsModified = false;

            await Db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<byte[]> GetPhoto(Guid id)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"No {nameof(Sheriff)} with Id: {id}");
            return savedSheriff.Photo;
        }

        public async Task<Sheriff> UpdateSheriffPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var savedSheriff = await Db.Sheriff.FirstOrDefaultAsync(s => (id.HasValue && s.Id == id) || (!id.HasValue && s.BadgeNumber == badgeNumber));
            savedSheriff.ThrowBusinessExceptionIfNull($"No {nameof(Sheriff)} with Badge: {badgeNumber} or Id: {id}");
            savedSheriff.Photo = photoData;
            savedSheriff.LastPhotoUpdate = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return savedSheriff;
        }

        public async Task UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {id} could not be found. ");
            savedSheriff.HomeLocation = await Db.Location.FindAsync(locationId);
            savedSheriff.HomeLocation.ThrowBusinessExceptionIfNull($"{nameof(Location)} with the id: {locationId} could not be found. ");
            await Db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(SheriffAwayLocation awayLocation)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);
            await ValidateNoOverlapAsync(awayLocation);

            awayLocation.Location = await Db.Location.FindAsync(awayLocation.LocationId);
            awayLocation.Sheriff = await Db.Sheriff.FindAsync(awayLocation.SheriffId);
            await Db.SheriffAwayLocation.AddAsync(awayLocation);
            await Db.SaveChangesAsync();
            return awayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(SheriffAwayLocation awayLocation)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);
            
            var savedAwayLocation = await Db.SheriffAwayLocation.FindAsync(awayLocation.Id);
            savedAwayLocation.ThrowBusinessExceptionIfNull($"{nameof(SheriffAwayLocation)} with the id: {awayLocation.Id} could not be found. ");

            if (savedAwayLocation.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffAwayLocation)} with the id: {awayLocation.Id} has been expired");

            await ValidateNoOverlapAsync(awayLocation, awayLocation.Id);

            Db.Entry(savedAwayLocation).CurrentValues.SetValues(awayLocation);
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryReason).IsModified = false;
            await Db.SaveChangesAsync();
            return savedAwayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            var sheriffAwayLocation = await Db.SheriffAwayLocation.FindAsync(id);
            sheriffAwayLocation.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffAwayLocation)} with the id: {id} could not be found. ");
            sheriffAwayLocation.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffAwayLocation.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Leave

        public async Task<SheriffLeave> AddSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);
            await ValidateNoOverlapAsync(sheriffLeave);

            sheriffLeave.LeaveType = await Db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.Sheriff = await Db.Sheriff.FindAsync(sheriffLeave.SheriffId);
            await Db.SheriffLeave.AddAsync(sheriffLeave);
            await Db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);

            var savedLeave = await Db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            if (savedLeave.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} has been expired");

            await ValidateNoOverlapAsync(sheriffLeave, sheriffLeave.Id);

            Db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);
            Db.Entry(savedLeave).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedLeave).Property(x => x.ExpiryReason).IsModified = false;
      
            await Db.SaveChangesAsync();
            return savedLeave;
        }

        public async Task RemoveSheriffLeave(int id, string expiryReason)
        {
            var sheriffLeave = await Db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            sheriffLeave.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffLeave.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Training

        public async Task<SheriffTraining> AddSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);
            await ValidateNoOverlapAsync(sheriffTraining);

            sheriffTraining.Sheriff = await Db.Sheriff.FindAsync(sheriffTraining.SheriffId);
            sheriffTraining.TrainingType = await Db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await Db.SheriffTraining.AddAsync(sheriffTraining);
            await Db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);
            
            var savedTraining = await Db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            if (savedTraining.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffTraining)} with the id: {sheriffTraining.Id} has been expired");

            await ValidateNoOverlapAsync(sheriffTraining, sheriffTraining.Id);

            Db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);
            Db.Entry(savedTraining).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedTraining).Property(x => x.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedTraining;
        }

        public async Task RemoveSheriffTraining(int id, string expiryReason)
        {
            var sheriffTraining = await Db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffTraining)} with the id: {id} could not be found. ");
            sheriffTraining.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffTraining.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        #region Validation
        private async Task CheckForBlankOrDuplicateIdirName(string idirName)
        {
            if (string.IsNullOrEmpty(idirName))
                throw new BusinessLayerException($"Missing {nameof(idirName)}.");

            var existingSheriffWithIdir = await Db.Sheriff.FirstOrDefaultAsync(s => s.IdirName.ToLower() == idirName.ToLower());
            if (existingSheriffWithIdir != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithIdir.LastName}, {existingSheriffWithIdir.FirstName} has IDIR name: {existingSheriffWithIdir.IdirName}");
        }

        private async Task CheckForBlankOrDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                throw new BusinessLayerException($"Missing {nameof(badgeNumber)}.");

            var existingSheriffWithBadge = await Db.Sheriff.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
            if (existingSheriffWithBadge != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithBadge.LastName}, {existingSheriffWithBadge.FirstName} already has badge number: {badgeNumber}");
        }

        private void ValidateStartAndEndDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate >= endDate) throw new BusinessLayerException("The from cannot be on or after the to. ");
        }

        private async Task ValidateSheriffExists(Guid sheriffId)
        {
            if (!await Db.Sheriff.AsNoTracking().AnyAsync(s => s.Id == sheriffId))
                throw new BusinessLayerException($"Sheriff with id: {sheriffId} does not exist.");
        }

        //TODO we may need this to span over more than it's provided type in the future. 
        private async Task ValidateNoOverlapAsync<T>(T data, int? updateOnlyId = null) where T : SheriffEvent
        {
            var entity = await Db.Set<T>().FirstOrDefaultAsync(sal =>
                sal.SheriffId == data.SheriffId && (sal.StartDate < data.EndDate && data.StartDate < sal.EndDate) &&
                sal.ExpiryDate == null && 
                (!updateOnlyId.HasValue ||
                 updateOnlyId.HasValue && sal.Id != updateOnlyId));

            if (entity == null)
                return;

            string startDateString;
            string endDateString;
            switch (data)
            {
                case SheriffAwayLocation sheriffAwayLocation:
                    //Calculate the start and end date for the away location.
                    var awayLocationTimezone = Db.Location.AsNoTracking().FirstOrDefault(al => al.Id == sheriffAwayLocation.LocationId)?.Timezone;
                    startDateString = entity.StartDate.ConvertToTimezone(awayLocationTimezone).ToString();
                    endDateString = entity.EndDate.ConvertToTimezone(awayLocationTimezone).ToString();
                    break;
                default:
                    //Calculate the start and end date for the user's home location id. 
                    var sheriffId = Db.Sheriff.AsNoTracking().FirstOrDefault(s => s.Id == data.SheriffId)?.HomeLocationId;
                    var homeLocationTimezone = Db.Location.AsNoTracking().FirstOrDefault(al => al.Id == sheriffId.Value)?.Timezone;
                    startDateString = entity.StartDate.ConvertToTimezone(homeLocationTimezone).ToString();
                    endDateString = entity.EndDate.ConvertToTimezone(homeLocationTimezone).ToString();
                    break;
            }
            throw new BusinessLayerException(
                $"Overlaps with existing {typeof(T).Name.ConvertCamelCaseToMultiWord()}: {startDateString} to {endDateString}");
        }

        
        #endregion

        #endregion
    }
}
