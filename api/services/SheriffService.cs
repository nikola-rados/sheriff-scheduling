using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto;
using SS.Db.models;
using SS.Db.models.auth;
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
            if (sheriff.IdirName.IsNullOrEmpty())
                throw new BusinessLayerException($"Missing {nameof(sheriff.IdirName)}.");

            await CheckForDuplicateIdirName(sheriff.IdirName);
            await CheckForDuplicateBadgeNumber(sheriff.BadgeNumber);

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

        public async Task<List<SheriffByLocationDto>> GetSheriffsForLocation(int? locationId)
        {
            var operation = DetermineProfileFilteringFromClaims();
            var currentUserId = User.CurrentUserId();
            var currentUserHomeLocationId = User.HomeLocationId();

            var fiveDaysFromNow = DateTimeOffset.Now.AddDays(5).Date;
            var now = DateTimeOffset.Now.Date;

            var sheriffQuery = _db.Sheriff.AsNoTracking()
                .AsSingleQuery()

                //Get sheriffs that are on Loan and have the matching home location, or all sheriffs.
                .Where(s => !locationId.HasValue ||
                            s.HomeLocationId == locationId || 
                            s.AwayLocation.Any(al => al.LocationId == locationId && !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                                                                                 && al.ExpiryDate == null))
                //Filter out our permissions. These may need some refining. 
                .Where(s => (operation == ViewProfileOperation.ViewProvince ||
                             (operation == ViewProfileOperation.ViewLocation &&
                              s.HomeLocationId == currentUserHomeLocationId ||
                              s.AwayLocation.Any(al =>
                                  al.LocationId == currentUserHomeLocationId &&
                                  !(al.StartDate > fiveDaysFromNow || now > al.EndDate) &&
                                  al.ExpiryDate == null)) || 
                             operation == ViewProfileOperation.ViewOwn && s.Id == currentUserId) &&
                            operation != ViewProfileOperation.None)

                //Include AwayLocation/Training/Leave that is within 5 days. 
                .Include(s => s.AwayLocation.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Training.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .Include(s => s.Leave.Where(al =>
                    !(al.StartDate > fiveDaysFromNow || now > al.EndDate)
                    && al.ExpiryDate == null))
                .Include(s => s.HomeLocation)
                .Include(s => s.UserRoles)
                .ThenInclude(ur => ur.Role);

            var sheriffs = await sheriffQuery.ToListAsync();

            var sheriffsByLocation = sheriffs.Adapt<List<SheriffByLocationDto>>();
            foreach (var sheriff in sheriffsByLocation)
            {
                //If LocationId = null, only show LoanedOuts. 
                sheriff.LoanedIn = sheriff.AwayLocation.Where(aw => locationId.HasValue && sheriff.HomeLocationId != locationId).SelectToList(
                        aw => $"Loaned from {aw.Location?.Name} {aw.StartDate} to {aw.EndDate}");
                sheriff.LoanedOut = sheriff.AwayLocation.Where(aw => !locationId.HasValue || sheriff.HomeLocationId == locationId).SelectToList(
                    aw => $"Loaned to {aw.Location?.Name} {aw.StartDate} to {aw.EndDate}");
            }

            return sheriffsByLocation;
        }

        public async Task<Sheriff> GetSheriff(Guid id)
        {
            var operation = DetermineProfileFilteringFromClaims();
            var currentUserId = User.CurrentUserId();
            var currentUserHomeLocationId = User.HomeLocationId();

            var today = DateTimeOffset.Now.Date;
            return await _db.Sheriff.AsNoTracking().AsSingleQuery()

                //Filter out our permissions. These may need some refining. 
                .Where(s => (operation == ViewProfileOperation.ViewProvince ||
                             (operation == ViewProfileOperation.ViewLocation &&
                              s.HomeLocationId == currentUserHomeLocationId ||
                              s.AwayLocation.Any(al =>
                                  al.LocationId == currentUserHomeLocationId &&
                                  al.EndDate >= today && al.ExpiryDate == null)) ||
                             operation == ViewProfileOperation.ViewOwn && s.Id == currentUserId) &&
                            operation != ViewProfileOperation.None)

                .Include(s=> s.HomeLocation)
                .Include(s => s.AwayLocation.Where (al => al.EndDate >= today && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Leave.Where(l => l.EndDate >= today && l.ExpiryDate == null))
                .Include(s => s.Training.Where(t => t.EndDate >= today && t.ExpiryDate == null))
                .Include(s => s.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sheriff> UpdateSheriff(Sheriff sheriff)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"Sheriff with the id: {sheriff.Id} could not be found. ");

            if (sheriff.BadgeNumber != savedSheriff.BadgeNumber)
                await CheckForDuplicateBadgeNumber(sheriff.BadgeNumber);

            _db.Entry(savedSheriff).CurrentValues.SetValues(sheriff);

            _db.Entry(savedSheriff).Property(x => x.IsEnabled).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.Photo).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.KeyCloakId).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.IdirId).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.IdirName).IsModified = false;
            _db.Entry(savedSheriff).Property(x => x.LastLogin).IsModified = false;

            await _db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<Sheriff> UpdateSheriffPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var savedSheriff = await _db.Sheriff.FirstOrDefaultAsync(s => (id.HasValue && s.Id == id) || (!id.HasValue && s.BadgeNumber == badgeNumber));
            savedSheriff.ThrowBusinessExceptionIfNull($"No sheriff with Badge: {badgeNumber} or Id: {id}");
            savedSheriff.Photo = photoData;
            await _db.SaveChangesAsync();
            return savedSheriff;
        }

        #endregion

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
            ValidateStartAndEndDates(sheriffAwayLocation.StartDate, sheriffAwayLocation.EndDate);
            ValidateSheriffExists(sheriffAwayLocation.SheriffId);
            ValidateNoAwayLocationOverlap(sheriffAwayLocation.SheriffId, sheriffAwayLocation.StartDate, sheriffAwayLocation.EndDate);

            sheriffAwayLocation.Location = await _db.Location.FindAsync(sheriffAwayLocation.LocationId);
            sheriffAwayLocation.Sheriff = await _db.Sheriff.FindAsync(sheriffAwayLocation.SheriffId);
            await _db.SheriffAwayLocation.AddAsync(sheriffAwayLocation);
            await _db.SaveChangesAsync();
            return sheriffAwayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
            ValidateStartAndEndDates(sheriffAwayLocation.StartDate, sheriffAwayLocation.EndDate);
            ValidateSheriffExists(sheriffAwayLocation.SheriffId);

            var savedAwayLocation = await _db.SheriffAwayLocation.FindAsync(sheriffAwayLocation.Id);
            savedAwayLocation.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffAwayLocation)} with the id: {sheriffAwayLocation.Id} could not be found. ");

            _db.Entry(savedAwayLocation).CurrentValues.SetValues(sheriffAwayLocation);
            await _db.SaveChangesAsync();
            return sheriffAwayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id)
        {
            var sheriffAwayLocation = await _db.SheriffAwayLocation.FindAsync(id);
            sheriffAwayLocation.ThrowBusinessExceptionIfNull(
                $"SheriffAwayLocation with the id: {id} could not be found. ");
            sheriffAwayLocation.ExpiryDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Leave

        public async Task<SheriffLeave> AddSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            ValidateSheriffExists(sheriffLeave.SheriffId);

            sheriffLeave.LeaveType = await _db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.Sheriff = await _db.Sheriff.FindAsync(sheriffLeave.SheriffId);
            await _db.SheriffLeave.AddAsync(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(SheriffLeave sheriffLeave)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            ValidateSheriffExists(sheriffLeave.SheriffId);

            var savedLeave = await _db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            _db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task RemoveSheriffLeave(int id)
        {
            var sheriffLeave = await _db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            sheriffLeave.ExpiryDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Training

        public async Task<SheriffTraining> AddSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            ValidateSheriffExists(sheriffTraining.SheriffId);

            sheriffTraining.Sheriff = await _db.Sheriff.FindAsync(sheriffTraining.SheriffId);
            sheriffTraining.TrainingType = await _db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await _db.SheriffTraining.AddAsync(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(SheriffTraining sheriffTraining)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            ValidateSheriffExists(sheriffTraining.SheriffId);

            var savedTraining = await _db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(savedTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            _db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task RemoveSheriffTraining(int id)
        {
            var sheriffTraining = await _db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(sheriffTraining)} with the id: {id} could not be found. ");
            sheriffTraining.ExpiryDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        private async Task CheckForDuplicateIdirName(string idirName)
        {
            if (string.IsNullOrEmpty(idirName))
                return;

            var existingSheriffWithIdir = await _db.Sheriff.FirstOrDefaultAsync(s => s.IdirName.ToLower() == idirName.ToLower());
            if (existingSheriffWithIdir != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithIdir.LastName}, {existingSheriffWithIdir.FirstName} has IDIR name: {existingSheriffWithIdir.IdirName}");
        }

        private async Task CheckForDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                return;

            var existingSheriffWithBadge = await _db.Sheriff.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
            if (existingSheriffWithBadge != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithBadge.LastName}, {existingSheriffWithBadge.FirstName} already has badge number: {badgeNumber}");
        }

        private ViewProfileOperation DetermineProfileFilteringFromClaims()
        {
            if (User.HasPermission(Permission.ViewProfilesInAllLocation))
                return ViewProfileOperation.ViewProvince;
            if (User.HasPermission(Permission.ViewProfilesInOwnLocation))
                return ViewProfileOperation.ViewLocation;
            if (User.HasPermission(Permission.ViewOwnProfile))
                return ViewProfileOperation.ViewOwn;
            return ViewProfileOperation.None;
        }

        private void ValidateStartAndEndDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate > endDate)
                throw new BusinessLayerException("The start date cannot be after the end date. ");
        }

        private void ValidateSheriffExists(Guid sheriffId)
        {
            if (!_db.Sheriff.AsNoTracking().Any(s => s.Id == sheriffId))
                throw new BusinessLayerException("Sheriff with id: {sheriffId} does not exist.");
        }

        private void ValidateNoAwayLocationOverlap(Guid sheriffId, DateTimeOffset startDate, DateTimeOffset endDate)
        {

        }
        #endregion
    }



    public enum ViewProfileOperation
    {
        ViewOwn,
        ViewLocation,
        ViewProvince,
        None
    };


}
