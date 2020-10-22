using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Db.models;

namespace SS.Api.services
{
    public class ScheduleService
    {
        private readonly SheriffDbContext _db;
        public ScheduleService(SheriffDbContext db)
        {
            _db = db;
        }

        public async Task<List<Shift>> GetShifts(int locationId)
        {
            //todo permission filtering
            return await _db.Shift.AsNoTracking().Where(s => s.LocationId == locationId).ToListAsync();
        }

        public async Task<Shift> AddShift(Shift entity)
        {
            await _db.Shift.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<Shift> UpdateShift(Shift entity)
        {
            var savedShift = await _db.Shift.FindAsync(entity.Id);
            savedShift.ThrowBusinessExceptionIfNull($"{typeof(Shift)} with the id: {entity.Id} could not be found. ");

            _db.Entry(savedShift).CurrentValues.SetValues(entity);

            //TODO optional fields we don't want saved?
            //_db.Entry(savedSheriff).Property(x => x.LastLogin).IsModified = false;

            await _db.SaveChangesAsync();
            return savedShift;
        }

        public async Task<Shift> AssignToShift(int id, List<Guid> sheriffIds)
        {
            var savedShift = await _db.Shift.FindAsync(id);
            savedShift.ThrowBusinessExceptionIfNull($"{typeof(Shift)} with the id: {id} could not be found. ");

            //Check for open slots.
            //Check for conflicts.
            //Create intermediate objects.

            return new Shift();
        }

        public async Task RemoveShift(int id)
        {
            var entity = _db.Shift.FirstOrDefault();
            entity.ThrowBusinessExceptionIfNull($"Shift with id: {id} could not be found.");
            entity!.ExpiryDate = DateTimeOffset.UtcNow;
            //Set ShiftSheriff to expired as well.
            await _db.SaveChangesAsync();
        }

        public async Task<List<Shift>> ImportShift(bool includeSheriffs)
        {
            //Check for conflicts.

            //Copy existing shifts / and ShiftSheriffs if includeSheriffs is toggled.

            await _db.SaveChangesAsync();
            return new List<Shift>();
        }
    }
}
