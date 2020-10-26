using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Db.models;
using SS.Db.models.scheduling;
using SS.Db.models.sheriff;

namespace SS.Api.services.scheduling
{
    public class ShiftService
    {
        private SheriffDbContext Db { get; }
        public ShiftService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<Shift>> GetShifts(int locationId)
        {
            return await Db.Shift.AsNoTracking()
                .Where(s => s.LocationId == locationId && s.ExpiryDate == null)
                .ToListAsync();
        }

        public async Task<Shift> AddShift(Shift entity)
        {
            entity.ExpiryDate = null;
            entity.Sheriff = null;
            entity.Location = await Db.Location.FindAsync(entity.LocationId);
            await Db.Shift.AddAsync(entity);
            await Db.SaveChangesAsync();
            return entity;
        }

        public async Task<Shift> UpdateShift(Shift entity)
        {
            var savedShift = await Db.Shift.FindAsync(entity.Id);
            savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {entity.Id} could not be found. ");

            //If assigned check if there are conflicts. 
            if (entity.SheriffId.HasValue)
            {
                var conflict = await CheckForOverlap(entity.StartDate.UtcDateTime, entity.EndDate.UtcDateTime,
                    entity.SheriffId.Value);
                conflict.ThrowBusinessExceptionIfNotEmpty(conflict);
            }

            Db.Entry(savedShift).CurrentValues.SetValues(entity);

            Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;
            //Db.Entry(savedShift).Property(x => x.Assignments).IsModified = false;
            //Db.Entry(savedShift).Property(x => x.AnticipatedAssignment).IsModified = false;
            Db.Entry(savedShift).Property(x => x.ExpiryDate).IsModified = false;

            await Db.SaveChangesAsync();
            return savedShift;
        }

        public async Task<Shift> AssignToShift(List<int> shiftIds, Guid sheriffId, bool overrideShift = false)
        {
            var savedSheriff = await Db.Sheriff.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sheriffId);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {sheriffId} could not be found.");

            var conflicts = new List<string>();
            foreach (var shiftId in shiftIds)
            {
                var savedShift = await Db.Shift.FindAsync(shiftId);
                savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {shiftId} could not be found. ");
                if (savedShift.ExpiryDate.HasValue)
                    throw new BusinessLayerException($"Trying to assign to an expired {nameof(Shift)}.");

                if (!overrideShift)
                    conflicts.Add(await CheckForOverlap(savedShift.StartDate.UtcDateTime, savedShift.EndDate.UtcDateTime, sheriffId));
            }

            conflicts.ThrowBusinessExceptionIfNotEmpty(conflicts.ListToStringWithPipes());

            foreach (var shiftId in shiftIds)
            {
                var savedShift = await Db.Shift.FindAsync(shiftId);
                savedShift.Sheriff = savedSheriff;
            }

            await Db.SaveChangesAsync();
            return new Shift();
        }

        public async Task RemoveShift(int id)
        {
            var entity = await Db.Shift.FirstOrDefaultAsync(s => s.Id == id);
            entity.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with id: {id} could not be found.");
            entity!.ExpiryDate = DateTimeOffset.UtcNow;
            await Db.SaveChangesAsync();
        }

        public async Task<List<Shift>> GetShiftsForSheriffs(IEnumerable<Guid> sheriffIds, DateTimeOffset startDate, DateTimeOffset endDate) =>
            await Db.Shift.AsNoTracking().Where(s =>
                    !(s.StartDate > endDate || startDate > s.EndDate) &&
                    s.SheriffId != null &&
                    sheriffIds.Contains((Guid)s.SheriffId) &&
                    s.ExpiryDate == null)
                .ToListAsync();

        public async Task<List<Shift>> ImportShifts(int locationId, bool includeSheriffs, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var shiftsToImport = Db.Shift.Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                                !(s.StartDate > endDate || startDate > s.EndDate));

            //Clone, move dates includeSheriffs

            await Db.SaveChangesAsync();
            return new List<Shift>();
        }

        #region Helpers
        public async Task<string> CheckForOverlap(DateTime start, DateTime end, Guid sheriffId)
        {
            var conflictingShift = await Db.Shift.AsNoTracking().AsSingleQuery()
                    .FirstOrDefaultAsync(s => !(s.StartDate > end || start > s.EndDate)
                                         && s.ExpiryDate == null &&
                                         s.SheriffId == sheriffId);

            if (conflictingShift == null) return null;
            var conflictingSheriff = await Db.Sheriff.AsNoTracking().FirstAsync(s => s.Id == sheriffId);
            return ConflictingSheriffAndSchedule(conflictingSheriff, conflictingShift);
        }

        private string ConflictingSheriffAndSchedule(Sheriff conflictingSheriff, Shift conflictingShift)
            => $"Conflict - {nameof(Sheriff)}: {conflictingSheriff.LastName}, {conflictingSheriff.FirstName} - Shift already exists: {conflictingShift}";


        #endregion
    }
}
