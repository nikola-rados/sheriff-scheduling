using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<Sheriff> DisableSheriff(Guid id)
        {
            var sheriff = await _db.Sheriff.FindAsync(id);
            if (sheriff == null)
                throw new BusinessLayerException($"Sheriff with the id: {id} could not be found. ");

            sheriff.IsDisabled = true;
            await _db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<Sheriff> EnableSheriff(Guid id)
        {
            var sheriff = await _db.Sheriff.FindAsync(id);
            if (sheriff == null)
                throw new BusinessLayerException($"Sheriff with the id: {id} could not be found. ");

            sheriff.IsDisabled = false;
            await _db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<List<Sheriff>> GetSheriffs(bool includeDisabled)
        {
            return await _db.Sheriff.Where(s => !includeDisabled || s.IsDisabled).ToListAsync();
        }

        public async Task<Sheriff> GetSheriff(Guid id)
        {
            return await _db.Sheriff.Include(s=> s.HomeLocation)
                .Include(s => s.AwayLocation)
                .Include(s => s.Leave)
                .Include(s => s.Training)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sheriff> UpdateBaseSheriff(Sheriff sheriff)
        {
            var savedSheriff = await _db.Sheriff.FindAsync(sheriff.Id);
            if (savedSheriff == null) throw new KeyNotFoundException($"{nameof(savedSheriff)} with the id: {savedSheriff.Id} could not be found. ");

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
            if (savedAwayLocation == null) throw new KeyNotFoundException($"{nameof(sheriffAwayLocation)} with the id: {sheriffAwayLocation.Id} could not be found. ");

            _db.Entry(savedAwayLocation).CurrentValues.SetValues(sheriffAwayLocation);

            await _db.SaveChangesAsync();
            return sheriffAwayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id)
        {
            var sheriffAwayLocation = await _db.SheriffAwayLocation.FindAsync(id);
            if (sheriffAwayLocation == null)
                throw new BusinessLayerException($"SheriffAwayLocation with the id: {id} could not be found. ");

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
            if (savedLeave == null) throw new KeyNotFoundException($"{nameof(sheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            _db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);

            await _db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task RemoveSheriffLeave(int id)
        {
            var sheriffLeave = await _db.SheriffLeave.FindAsync(id);
            if (sheriffLeave == null)
                throw new BusinessLayerException($"SheriffLeave with the id: {id} could not be found. ");

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
            var savedLeave = await _db.SheriffTraining.FindAsync(sheriffTraining.Id);
            if (savedLeave == null) throw new KeyNotFoundException($"{nameof(sheriffTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            _db.Entry(savedLeave).CurrentValues.SetValues(sheriffTraining);

            await _db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task RemoveSheriffTraining(int id)
        {
            var sheriffTraining = await _db.SheriffTraining.FindAsync(id);
            if (sheriffTraining == null)
                throw new BusinessLayerException($"SheriffTraining with the id: {id} could not be found. ");

            _db.SheriffTraining.Remove(sheriffTraining);
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
