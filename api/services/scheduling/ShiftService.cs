using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Db.models;
using SS.Db.models.scheduling;
using SS.Db.models.sheriff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.services.scheduling
{
    public class ShiftService
    {
        private SheriffDbContext Db { get; }
        public ShiftService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<Shift>> GetShifts(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            return await Db.Shift.AsNoTracking()
                .Include( s=> s.Location)
                .Include(s => s.Sheriff)
                .Include(s => s.AnticipatedAssignment)
                .Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                            !(s.StartDate > end || start > s.EndDate))
                .ToListAsync();
        }

        public async Task<Shift> AddShift(Shift entity)
        {
            entity.Duties = null;
            entity.ExpiryDate = null;
            entity.Sheriff = await Db.Sheriff.FindAsync(entity.SheriffId);
            entity.AnticipatedAssignment = await Db.Assignment.FindAsync(entity.AnticipatedAssignmentId);
            entity.Location = await Db.Location.FindAsync(entity.LocationId);
            entity.Location.ThrowBusinessExceptionIfNull(
                $"{nameof(Location)} with id: {entity.LocationId} does not exist.");
            await Db.Shift.AddAsync(entity);
            await Db.SaveChangesAsync();
            return entity;
        }

        public async Task<Shift> UpdateShift(Shift entity)
        {
            var savedShift = await Db.Shift.FindAsync(entity.Id);
            savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {entity.Id} could not be found.");

            if (entity.SheriffId.HasValue && entity.SheriffId != savedShift.SheriffId)
            {
                var conflict = await CheckForConflictForSingleShift(entity.StartDate, entity.EndDate, entity.SheriffId.Value);
                conflict.ThrowBusinessExceptionIfNotNull(conflict);
            }

            Db.Entry(savedShift).CurrentValues.SetValues(entity);

            Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;
            Db.Entry(savedShift).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;

            savedShift.Sheriff = await Db.Sheriff.FindAsync(entity.SheriffId);
            savedShift.AnticipatedAssignment = await Db.Assignment.FindAsync(entity.AnticipatedAssignmentId);

            await Db.SaveChangesAsync();
            return savedShift;
        }

        public async Task<List<Shift>> AssignToShifts(List<int> shiftIds, Guid sheriffId, bool overrideShift = false)
        {
            var savedSheriff = await Db.Sheriff.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sheriffId);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {sheriffId} could not be found.");

            var savedShifts = Db.Shift.Where(s => shiftIds.Contains(s.Id));
            if (savedShifts.Any(ss => ss.ExpiryDate != null))
                throw new BusinessLayerException("Shift(s) attempting to be scheduled have been expired.");

            var locationId = savedShifts.First().LocationId;

            if (savedShifts.Any(a =>
                savedShifts.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate)))
                throw new BusinessLayerException("Shifts provided overlap with themselves.");


            var overlappingShifts = Db.Shift.AsNoTracking()
                .Where(a =>
                    a.ExpiryDate == null &&
                    a.LocationId == locationId &&
                    a.SheriffId == sheriffId &&
                    savedShifts.Any(b =>
                        b.ExpiryDate == null && a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate));

            if (overrideShift)
            {
                await overlappingShifts.ForEachAsync(s => s.SheriffId = null);
                overlappingShifts = Enumerable.Empty<Shift>().AsQueryable();
            }

            if (overlappingShifts.Any())
            {
                var message = overlappingShifts.Select(ol => ConflictingSheriffAndSchedule(savedSheriff, ol)).ToList()
                    .ListToStringWithPipes();
                throw new BusinessLayerException(message);
            }

            await savedShifts.ForEachAsync(s => s.SheriffId = sheriffId);
            await Db.SaveChangesAsync();

            return await savedShifts.ToListAsync();
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

        public async Task<List<Shift>> ImportWeeklyShifts(int locationId, bool includeSheriffs)
        {
            var location = Db.Location.FirstOrDefault(l => l.Id == locationId);
            location.ThrowBusinessExceptionIfNull($"Couldn't find {nameof(Location)} with id: {locationId}.");
            var timezone = location?.Timezone;
            timezone.ThrowBusinessExceptionIfNull("Timezone was null.");

            var lastMondayStart = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(-(int) DateTime.Now.DayOfWeek - 6));

            //We need to adjust to their start of the week, because it can differ depending on the TZ! 
            var targetStartDate = lastMondayStart.ConvertToTimezone(timezone);
            var targetEndDate = lastMondayStart.TranslateDateIfDaylightSavings(timezone, 7);

            //may need to refine this date query
            var shiftsToImport = Db.Shift
                .Include(s => s.Location)
                .AsNoTracking()
                .Where(s => s.LocationId == locationId &&
                            s.ExpiryDate == null &&
                            !(s.StartDate > targetEndDate || targetStartDate > s.EndDate));

            var importedShifts = new List<Shift>();
            foreach (var importShift in shiftsToImport)
            {
                var shift = Db.DetachedClone(importShift);
                shift.Id = 0;
                shift.SheriffId = includeSheriffs ? shift.SheriffId : null;
                shift.LocationId = importShift.LocationId;
                shift.StartDate = shift.StartDate.TranslateDateIfDaylightSavings(timezone, 7);
                shift.EndDate = shift.EndDate.TranslateDateIfDaylightSavings(timezone, 7);
                await Db.Shift.AddAsync(shift);
                importedShifts.Add(shift);
            }

            await Db.SaveChangesAsync();
            return importedShifts;
        }

        #region Helpers

        private static string ConflictingSheriffAndSchedule(Sheriff sheriff, Shift shift)
            => $"Conflict - {nameof(Sheriff)}: {sheriff.LastName}, {sheriff.FirstName} - Existing Shift conflicts: {shift.StartDate.ConvertToTimezone(shift.Timezone)} -> {shift.EndDate.ConvertToTimezone(shift.Timezone)}";

        private async Task<string> CheckForConflictForSingleShift(DateTimeOffset start, DateTimeOffset end, Guid sheriffId)
        {
            var conflictingShift = await Db.Shift.AsNoTracking()
                .AsSingleQuery()
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => (s.StartDate < end && start < s.EndDate) &&
                                          s.ExpiryDate == null &&
                                          s.SheriffId == sheriffId);

            if (conflictingShift == null)
                return null;

            var conflictingSheriff = await Db.Sheriff.AsNoTracking()
                .AsSingleQuery()
                .FirstAsync(s => s.Id == sheriffId);

            return ConflictingSheriffAndSchedule(conflictingSheriff, conflictingShift);
        }


        #endregion
    }
}
