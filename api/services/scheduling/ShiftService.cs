using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Api.services.usermanagement;
using SS.Common.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using SS.Db.models.scheduling.notmapped;
using SS.Db.models.sheriff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.models;

namespace SS.Api.services.scheduling
{
    public class ShiftService
    {
        private SheriffDbContext Db { get; }
        private SheriffService SheriffService { get; }
        public ShiftService(SheriffDbContext db, SheriffService sheriffService)
        {
            Db = db;
            SheriffService = sheriffService;
        }

        public async Task<List<Shift>> GetShifts(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            return await Db.Shift.AsSingleQuery().AsNoTracking()
                .Include( s=> s.Location)
                .Include(s => s.Sheriff)
                .Include(s => s.AnticipatedAssignment)
                .Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                            s.StartDate < end && start < s.EndDate)
                .ToListAsync();
        }

        public async Task<List<Shift>> AddShifts(List<Shift> shifts)
        {
            var overlaps = await GetShiftConflicts(shifts);
            if (overlaps.Any()) throw new BusinessLayerException(overlaps.SelectMany(ol => ol.ConflictMessages).ToStringWithPipes());

            foreach (var shift in shifts)
            {
                if (shift.StartDate > shift.EndDate) throw new BusinessLayerException($"{nameof(Shift)} Start date cannot come after end date.");
                shift.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(shift.Timezone)} needs to be included in the {nameof(Shift)}.");
                shift.Duties = null;
                shift.ExpiryDate = null;
                shift.Sheriff = await Db.Sheriff.FindAsync(shift.SheriffId);
                shift.AnticipatedAssignment = await Db.Assignment.FindAsync(shift.AnticipatedAssignmentId);
                shift.Location = await Db.Location.FindAsync(shift.LocationId);
                await Db.Shift.AddAsync(shift);
            }
            await Db.SaveChangesAsync();
            return shifts;
        }

        public async Task<List<Shift>> UpdateShifts(List<Shift> shifts)
        {
            var overlaps = await GetShiftConflicts(shifts);
            if (overlaps.Any()) throw new BusinessLayerException(overlaps.SelectMany(ol => ol.ConflictMessages).ToStringWithPipes());

            var shiftIds = shifts.SelectToList(s => s.Id);
            var savedShifts = Db.Shift.Where(s => shiftIds.Contains(s.Id));

            foreach (var shift in shifts)
            {
                var savedShift = savedShifts.FirstOrDefault(s => s.Id == shift.Id);
                savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {shift.Id} could not be found.");
                if (shift.StartDate > shift.EndDate) throw new BusinessLayerException($"{nameof(Shift)} Start date cannot come after end date.");
                shift.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(shift.Timezone)} needs to be included in the {nameof(Shift)}.");

                Db.Entry(savedShift!).CurrentValues.SetValues(shift);
                Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;
                Db.Entry(savedShift).Property(x => x.ExpiryDate).IsModified = false;

                savedShift.Sheriff = await Db.Sheriff.FindAsync(shift.SheriffId);
                savedShift.AnticipatedAssignment = await Db.Assignment.FindAsync(shift.AnticipatedAssignmentId);
            }

            await Db.SaveChangesAsync();
            return await savedShifts.ToListAsync();
        }

        public async Task ExpireShifts(List<int> ids)
        {
            foreach (var id in ids)
            {
                var entity = await Db.Shift.FirstOrDefaultAsync(s => s.Id == id);
                entity.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with id: {id} could not be found.");
                entity!.ExpiryDate = DateTimeOffset.UtcNow;
                var dutySlots = Db.DutySlot.Where(d => d.ShiftId == id);
                await dutySlots.ForEachAsync(ds =>
                {
                    ds.SheriffId = null;
                    ds.ShiftId = null;
                });
            }
            await Db.SaveChangesAsync();
        }

        public async Task<ImportedShifts> ImportWeeklyShifts(int locationId, bool includeSheriffs, DateTimeOffset start)
        {
            var location = Db.Location.FirstOrDefault(l => l.Id == locationId);
            location.ThrowBusinessExceptionIfNull($"Couldn't find {nameof(Location)} with id: {locationId}.");
            var timezone = location?.Timezone;
            timezone.GetTimezone().ThrowBusinessExceptionIfNull("Timezone was invalid.");

            //We need to adjust to their start of the week, because it can differ depending on the TZ! 
            var targetStartDate = start.ConvertToTimezone(timezone);
            var targetEndDate = targetStartDate.TranslateDateIfDaylightSavings(timezone, 7);

            var shiftsToImport = Db.Shift
                .Include(s => s.Location)
                .AsNoTracking()
                .Where(s => s.LocationId == locationId &&
                            s.ExpiryDate == null &&
                            s.StartDate < targetEndDate && targetStartDate < s.EndDate);  

            var importedShifts = await shiftsToImport.Select(shift => Db.DetachedClone(shift)).ToListAsync();
            foreach (var shift in importedShifts)
            {
                shift.Id = 0;
                shift.SheriffId = includeSheriffs ? shift.SheriffId : null;
                shift.StartDate = shift.StartDate.TranslateDateIfDaylightSavings(timezone, 7);
                shift.EndDate = shift.EndDate.TranslateDateIfDaylightSavings(timezone, 7);
            }

            var overlaps = await GetShiftConflicts(importedShifts);
            var filteredImportedShifts = importedShifts.WhereToList(s => overlaps.All(o => o.Shift.Id != s.Id));

            await Db.Shift.AddRangeAsync(filteredImportedShifts);
            await Db.SaveChangesAsync();

            return new ImportedShifts
            {
                ConflictMessages = overlaps.SelectMany(o => o.ConflictMessages).ToList(),
                Shifts = filteredImportedShifts
            };
        }

        public async Task<List<ShiftAvailability>> GetShiftAvailability(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var sheriffs = await SheriffService.GetSheriffsForShiftAvailability(locationId, start, end);
            var shiftsForSheriffs = await GetShiftsForSheriffs(sheriffs.Select(s => s.Id), start, end);

            var sheriffEventConflicts = new List<ShiftAvailabilityConflict>();
            sheriffs.ForEach(sheriff =>
            {
                sheriffEventConflicts.AddRange(sheriff.AwayLocation.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.AwayLocation, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate,
                    End = s.EndDate, 
                    LocationId = s.LocationId,
                    Location = s.Location
                }));
                sheriffEventConflicts.AddRange(sheriff.Leave.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Leave, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate, 
                    End = s.EndDate
                }));
                sheriffEventConflicts.AddRange(sheriff.Training.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Training, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate, 
                    End = s.EndDate
                }));
            });

            var existingShiftConflicts = shiftsForSheriffs.Select(s => new ShiftAvailabilityConflict
            {
                Conflict = ShiftConflictType.Scheduled, 
                SheriffId = s.SheriffId, 
                Location = s.Location, 
                LocationId = s.LocationId, 
                Start = s.StartDate, 
                End = s.EndDate, 
                ShiftId = s.Id
            });

            var allShiftConflicts = sheriffEventConflicts.Concat(existingShiftConflicts).ToList();
            
            return sheriffs.SelectToList(s => new ShiftAvailability
            {
                Start = start,
                End = end,
                Sheriff = s,
                SheriffId = s.Id,
                Conflicts = allShiftConflicts.WhereToList(asc => asc.SheriffId == s.Id)
            }).ToList();
        }

        #region Helpers

        #region Availability

        private async Task<List<ShiftConflict>> GetShiftConflicts(List<Shift> shifts)
        {
            var overlappingShifts = await CheckForShiftOverlap(shifts);
            var sheriffEventOverlaps = await CheckSheriffEventsOverlap(shifts);
            return overlappingShifts.Concat(sheriffEventOverlaps).ToList();
        }

        private async Task<List<ShiftConflict>> CheckForShiftOverlap(List<Shift> shifts)
        {
            var overlappingShifts = await OverlappingShifts(shifts);
            return overlappingShifts.SelectToList(ol => new ShiftConflict
            {
                ConflictMessages = new List<string>
                {
                    ConflictingSheriffAndSchedule(ol.Sheriff, ol)
                },
                Shift = ol
            });
        }

        private async Task<List<Shift>> OverlappingShifts(List<Shift> targetShifts)
        {
            if (!targetShifts.Any()) throw new BusinessLayerException("No shifts were provided.");
            if (targetShifts.Any(a =>
                targetShifts.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate && a.SheriffId == b.SheriffId)))
                throw new BusinessLayerException("Shifts provided overlap with themselves.");

            var sheriffIds = targetShifts.Select(ts => ts.SheriffId).Distinct();
            var locationId = targetShifts.First().LocationId;

            var conflictingShifts = new List<Shift>();
            foreach (var ts in targetShifts)
            {
                conflictingShifts.AddRange(await Db.Shift.AsNoTracking()
                    .Include(s => s.Sheriff)
                    .Where(s =>
                        s.ExpiryDate == null &&
                        s.LocationId == locationId &&
                        s.StartDate < ts.EndDate && ts.StartDate < s.EndDate &&
                        sheriffIds.Contains(s.SheriffId)
                    ).ToListAsync());
            }

            conflictingShifts = conflictingShifts.Distinct().WhereToList(s =>
                targetShifts.Any(ts =>
                    ts.ExpiryDate == null && s.Id != ts.Id && ts.StartDate < s.EndDate && s.StartDate < ts.EndDate &&
                    ts.SheriffId == s.SheriffId) &&
                targetShifts.All(ts => ts.Id != s.Id)
            );

            return conflictingShifts;
        }

        private async Task<List<ShiftConflict>> CheckSheriffEventsOverlap(List<Shift> shifts)
        {
            var sheriffEventConflicts = new List<ShiftConflict>();
            foreach (var shift in shifts)
            {
                var locationId = shift.LocationId;
                var sheriffs = await SheriffService.GetSheriffsForShiftAvailability(locationId, shift.StartDate, shift.EndDate, shift.SheriffId);
                var sheriff = sheriffs.FirstOrDefault();
                sheriff.ThrowBusinessExceptionIfNull($"Couldn't find active {nameof(Sheriff)}, they might not be active in location for the shift.");
                var validationErrors = new List<string>();
                validationErrors.AddRange(sheriff!.AwayLocation.Where(aw => aw.LocationId != shift.LocationId).Select(aw => PrintSheriffEventConflict<SheriffAwayLocation>(aw.Sheriff, aw.StartDate, aw.EndDate)));
                validationErrors.AddRange(sheriff.Leave.Select(aw => PrintSheriffEventConflict<SheriffLeave>(aw.Sheriff, aw.StartDate, aw.EndDate)));
                validationErrors.AddRange(sheriff.Training.Select(aw => PrintSheriffEventConflict<SheriffTraining>(aw.Sheriff, aw.StartDate, aw.EndDate)));

                if (validationErrors.Any())
                    sheriffEventConflicts.Add(new ShiftConflict
                    {
                        Shift = shift,
                        ConflictMessages = validationErrors
                    });
            }
            return sheriffEventConflicts;
        }

        private async Task<List<Shift>> GetShiftsForSheriffs(IEnumerable<Guid> sheriffIds, DateTimeOffset startDate, DateTimeOffset endDate) =>
            await Db.Shift.AsSingleQuery().AsNoTracking()
                    .Include(s => s.Location)
                    .Where(s =>
                        s.StartDate < endDate && startDate < s.EndDate &&
                        s.SheriffId != null &&
                        sheriffIds.Contains((Guid)s.SheriffId) &&
                        s.ExpiryDate == null)
                    .ToListAsync();

        #endregion Availability

        #region String Helpers
        private static string ConflictingSheriffAndSchedule(Sheriff sheriff, Shift shift)
            => $"Conflict - {nameof(Sheriff)}: {sheriff.LastName}, {sheriff.FirstName} - Existing Shift conflicts: {shift.StartDate.ConvertToTimezone(shift.Timezone)} -> {shift.EndDate.ConvertToTimezone(shift.Timezone)}";

        private static string PrintSheriffEventConflict<T>(Sheriff sheriff, DateTimeOffset start, DateTimeOffset end)
            => $"{sheriff.LastName}, {sheriff.FirstName} has a(n) {typeof(T).Name.Replace("Sheriff", "").ConvertCamelCaseToMultiWord()} from: {start} to: {end}";
        #endregion String Helpers

        #endregion
    }
}
