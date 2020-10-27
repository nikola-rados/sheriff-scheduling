using Microsoft.EntityFrameworkCore;
using NodaTime;
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
            savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {entity.Id} could not be found. ");

            if (entity.SheriffId.HasValue && entity.SheriffId != savedShift.SheriffId)
            {
                //TODO check for conflicts?
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

        public async Task AssignToShifts(List<int> shiftIds, Guid sheriffId, bool overrideShift = false)
        {
            var savedSheriff = await Db.Sheriff.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sheriffId);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {sheriffId} could not be found.");

            var savedShifts = Db.Shift.Where(s => shiftIds.Contains(s.Id) && s.ExpiryDate == null);

            if (savedShifts.Count() != shiftIds.Count)
                throw new BusinessLayerException("Couldn't find all shift ids provided.");
          
            //TODO check for conflicts?

            foreach (var savedShift in savedShifts)
                savedShift.Sheriff = savedSheriff;

            await Db.SaveChangesAsync();
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

            var copyStart = DateTime.Today.AddDays(-(int) DateTime.Now.DayOfWeek - 6);
            var copyEnd = copyStart.AddDays(7);
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone!];

            //We need to adjust to their start of the week, because it can differ depending on the TZ! 
            var startOfWeekInTz = Instant.FromDateTimeOffset(copyStart).InZone(locationTimeZone);
            var endOfWeekInTz = Instant.FromDateTimeOffset(copyEnd).InZone(locationTimeZone);

            var targetStartDate = startOfWeekInTz.ToDateTimeOffset();
            var targetEndDate = endOfWeekInTz.ToDateTimeOffset();

            var shiftsToImport = Db.Shift
                .Include(s=> s.Location).AsNoTracking().Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                                !(s.StartDate > targetEndDate || targetStartDate > s.EndDate));

            var importedShifts = new List<Shift>();
            foreach (var importShift in shiftsToImport)
            {
                //We aren't duplicating duties here, they're scheduled closer to the date. 
                var shift = Db.DetachedClone(importShift);
                shift.Id = 0;
                shift.Duties = new List<Duty>();
                shift.SheriffId = includeSheriffs ? shift.SheriffId : null;
                shift.LocationId = importShift.LocationId;
                shift.StartDate = TranslateDateIfDaylightSavings(shift.StartDate, importShift.Location.Timezone, 7).ToDateTimeOffset();
                shift.EndDate = TranslateDateIfDaylightSavings(shift.StartDate, importShift.Location.Timezone, 7).ToDateTimeOffset();
                await Db.Shift.AddAsync(shift);
                importedShifts.Add(shift);
            }

            await Db.SaveChangesAsync();
            return importedShifts;
        }

        #region Helpers

        public ZonedDateTime TranslateDateIfDaylightSavings(DateTimeOffset date, string timezone, int daysToShift)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];

            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            var movedZoned = zoned.Plus(Duration.FromDays(daysToShift));

            if (movedZoned.Offset > zoned.Offset)
                movedZoned =
                    movedZoned.PlusHours(zoned.Offset.ToTimeSpan().Hours - movedZoned.Offset.ToTimeSpan().Hours);
            else if (movedZoned.Offset < zoned.Offset)
                movedZoned =
                    movedZoned.PlusHours(movedZoned.Offset.ToTimeSpan().Hours - zoned.Offset.ToTimeSpan().Hours);
            return movedZoned;
        }

        #endregion
    }
}
