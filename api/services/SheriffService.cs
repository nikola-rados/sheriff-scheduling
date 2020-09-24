using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _db.Sheriff.Include(s => s.AwayLocations)
                .Include(s => s.Leaves)
                .Include(s => s.Training)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sheriff> UpdateSheriff(Sheriff sheriff)
        {
            _db.Sheriff.Update(sheriff);
            await _db.SaveChangesAsync();
            return sheriff;
        }

        #endregion

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
            await _db.SheriffAwayLocation.AddAsync(sheriffAwayLocation);
            await _db.SaveChangesAsync();
            return sheriffAwayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(SheriffAwayLocation sheriffAwayLocation)
        {
            _db.SheriffAwayLocation.Update(sheriffAwayLocation);
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

        public async Task<int> AddSheriffLeave(SheriffLeave sheriffLeave)
        {
            await _db.SheriffLeave.AddAsync(sheriffLeave);
            await _db.SaveChangesAsync();
            return sheriffLeave.Id;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(SheriffLeave sheriffLeave)
        {
            _db.SheriffLeave.Update(sheriffLeave);
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

        public async Task<int> AddSheriffTraining(SheriffTraining sheriffTraining)
        {
            await _db.SheriffTraining.AddAsync(sheriffTraining);
            await _db.SaveChangesAsync();
            return sheriffTraining.Id;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(SheriffTraining sheriffTraining)
        {
            _db.SheriffTraining.Update(sheriffTraining);
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
