using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
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
        private readonly SheriffDbContext _db;
        public SheriffService(SheriffDbContext db)
        {
            _db = db;
        }

        #region Sheriff

        public async Task<Sheriff> CreateSheriff(Sheriff sheriff)
        {
            if (sheriff.IdirName.IsNullOrEmpty())
                throw new BusinessLayerException($"Missing {nameof(sheriff.IdirName)}.");

            await CheckForDuplicateBadgeNumber(sheriff.BadgeNumber);

            sheriff.AwayLocation = null;
            sheriff.Training = null;
            sheriff.Leave = null;
            sheriff.HomeLocation = await _db.Location.FindAsync(sheriff.HomeLocationId);
            sheriff.IsEnabled = true;
            await _db.Sheriff.AddAsync(sheriff);
            await _db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<List<Sheriff>> GetSheriffs(int locationId)
        {
            return await _db.Sheriff.Where(s => s.HomeLocationId == locationId && s.IsEnabled).ToListAsync();
        }

        public async Task<Sheriff> GetSheriff(Guid id)
        {
            return await _db.Sheriff.Include(s=> s.HomeLocation)
                .Include(s => s.AwayLocation)
                .Include(s => s.Leave)
                .Include(s => s.Training)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sheriff> UpdateSheriff(Sheriff sheriff)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"Sheriff with the id: {sheriff.Id} could not be found. ");

            if (sheriff.BadgeNumber != savedSheriff.BadgeNumber)
            {
                await CheckForDuplicateBadgeNumber(sheriff.BadgeNumber);
            }

            _db.Entry(savedSheriff).CurrentValues.SetValues(sheriff);

            await _db.SaveChangesAsync();
            return sheriff;
        }

        #endregion

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
            sheriffAwayLocation.Location = await _db.Location.FindAsync(sheriffAwayLocation.LocationId);
            sheriffAwayLocation.Sheriff = await _db.Sheriff.FindAsync(sheriffAwayLocation.SheriffId);
            await _db.SheriffAwayLocation.AddAsync(sheriffAwayLocation);
            await _db.SaveChangesAsync();
            return sheriffAwayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
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
            sheriffAwayLocation.ThrowBusinessExceptionIfNull($"SheriffAwayLocation with the id: {id} could not be found. ");

            _db.SheriffAwayLocation.Remove(sheriffAwayLocation);
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Leave

        public async Task<SheriffLeave> AddSheriffLeave(SheriffLeave sheriffLeave)
        {
            sheriffLeave.LeaveType = await _db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.Sheriff = await _db.Sheriff.FindAsync(sheriffLeave.SheriffId);
            await _db.SheriffLeave.AddAsync(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(SheriffLeave sheriffLeave)
        {
            var savedLeave = await _db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull($"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            _db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);

            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task RemoveSheriffLeave(int id)
        {
            var sheriffLeave = await _db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull($"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            _db.SheriffLeave.Remove(sheriffLeave);
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Sheriff Training

        public async Task<SheriffTraining> AddSheriffTraining(SheriffTraining sheriffTraining)
        {
            sheriffTraining.Sheriff = await _db.Sheriff.FindAsync(sheriffTraining.SheriffId);
            sheriffTraining.TrainingType = await _db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await _db.SheriffTraining.AddAsync(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(SheriffTraining sheriffTraining)
        {
            var savedTraining = await _db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull($"{nameof(savedTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            _db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);

            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task RemoveSheriffTraining(int id)
        {
            var sheriffTraining = await _db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull($"{nameof(sheriffTraining)} with the id: {id} could not be found. ");

            _db.SheriffTraining.Remove(sheriffTraining);
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        private async Task CheckForDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                return;

            var existingSheriffWithBadge = await _db.Sheriff.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
            if (existingSheriffWithBadge != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithBadge.LastName}, {existingSheriffWithBadge.FirstName} already has badge number: {badgeNumber}");
        }
        #endregion
    }
}
