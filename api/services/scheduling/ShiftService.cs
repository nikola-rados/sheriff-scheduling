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
using SS.Api.services.usermanagement;
using SS.Common.helpers.extensions;
using SS.Db.models.scheduling.notmapped;

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
            return await Db.Shift.AsNoTracking()
                .Include( s=> s.Location)
                .Include(s => s.Sheriff)
                .Include(s => s.AnticipatedAssignment)
                .Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                            !(s.StartDate > end || start > s.EndDate)) //May need to change this. This includes times on the edge.
                .ToListAsync();
        }

        public async Task<List<Shift>> AddShifts(List<Shift> shifts)
        {
            await CheckForShiftOverlap(shifts);
            await CheckSheriffEvents(shifts);

            foreach (var shift in shifts)
            {
                shift.Timezone.ThrowBusinessExceptionIfNullOrEmpty($"{nameof(shift.Timezone)} is a required field.");
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
            await CheckForShiftOverlap(shifts);
            await CheckSheriffEvents(shifts);

            var shiftIds = shifts.SelectToList(s => s.Id);
            var savedShifts = Db.Shift.Where(s => shiftIds.Contains(s.Id));

            foreach (var shift in shifts)
            {
                var savedShift = savedShifts.FirstOrDefault(s => s.Id == shift.Id);
                savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {shift.Id} could not be found.");
                shift.Timezone.ThrowBusinessExceptionIfNullOrEmpty($"{nameof(shift.Timezone)} is a required field.");

                Db.Entry(savedShift!).CurrentValues.SetValues(shift);
                Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;
                Db.Entry(savedShift).Property(x => x.ExpiryDate).IsModified = false;

                savedShift.Sheriff = await Db.Sheriff.FindAsync(shift.SheriffId);
                savedShift.AnticipatedAssignment = await Db.Assignment.FindAsync(shift.AnticipatedAssignmentId);
            }

            await Db.SaveChangesAsync();
            return await savedShifts.ToListAsync();
        }

        public async Task ExpireShift(int id)
        {
            var entity = await Db.Shift.FirstOrDefaultAsync(s => s.Id == id);
            entity.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with id: {id} could not be found.");
            entity!.ExpiryDate = DateTimeOffset.UtcNow;
            await Db.SaveChangesAsync();
        }

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

            var shiftsToImport = Db.Shift
                .Include(s => s.Location)
                .AsNoTracking()
                .Where(s => s.LocationId == locationId &&
                            s.ExpiryDate == null &&
                            !(s.StartDate > targetEndDate || targetStartDate > s.EndDate));   //may need to refine this date query - This includes times on the edge.

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

        public async Task<List<ShiftAvailability>> GetShiftAvailability(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var sheriffs = await SheriffService.GetSheriffsForShiftAvailability(locationId, start, end);
            var shiftsForSheriffs = await GetShiftsForSheriffs(sheriffs.Select(s => s.Id), start, end);

            var sheriffEventConflicts = new List<ShiftConflict>();
            sheriffs.ForEach(sheriff =>
            {
                sheriffEventConflicts.AddRange(sheriff.AwayLocation.Select(s => new ShiftConflict
                {
                    Conflict = ShiftConflictType.AwayLocation, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate,
                    End = s.EndDate, 
                    LocationId = s.LocationId
                }));
                sheriffEventConflicts.AddRange(sheriff.Leave.Select(s => new ShiftConflict
                {
                    Conflict = ShiftConflictType.Leave, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate, 
                    End = s.EndDate
                }));
                sheriffEventConflicts.AddRange(sheriff.Training.Select(s => new ShiftConflict
                {
                    Conflict = ShiftConflictType.Training, 
                    SheriffId = sheriff.Id, 
                    Start = s.StartDate, 
                    End = s.EndDate
                }));
            });

            var existingShiftConflicts = shiftsForSheriffs.Select(s => new ShiftConflict
                {Conflict = ShiftConflictType.Scheduled, SheriffId = s.SheriffId, LocationId = s.LocationId, Start = s.StartDate, End = s.EndDate});

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

        //todo In Progress
        public async Task AdjustShift(int shiftId, DateTimeOffset? start = null, DateTimeOffset? end = null)
        {
            var savedShift = await Db.Shift.FindAsync(shiftId);
            //Check for conflicts. Vacation / Leave / New Location etc.
            savedShift.StartDate = start ?? savedShift.StartDate;
            savedShift.EndDate = end ?? savedShift.EndDate;
            await Db.SaveChangesAsync();
        }

        #region Helpers
        private async Task<List<Shift>> GetShiftsForSheriffs(IEnumerable<Guid> sheriffIds, DateTimeOffset startDate, DateTimeOffset endDate) =>
            await Db.Shift.AsNoTracking().Where(s =>
                    !(s.StartDate > endDate || startDate > s.EndDate) && //Date may require refining -  This includes times on the edge.
                    s.SheriffId != null &&
                    sheriffIds.Contains((Guid)s.SheriffId) &&
                    s.ExpiryDate == null)
                .ToListAsync();

        #region Availability
        private async Task CheckSheriffEvents(List<Shift> shifts)
        {
            var validationErrors = new List<string>();

            foreach (var shift in shifts)
            {
                var locationId = shift.LocationId;
                //This is the same call our availability uses. 
                var sheriffs = await SheriffService.GetSheriffsForShiftAvailability(locationId, shift.StartDate, shift.EndDate, shift.SheriffId);
                var sheriff = sheriffs.FirstOrDefault();
                sheriff.ThrowBusinessExceptionIfNull($"Couldn't find active {nameof(Sheriff)}, they might not be active in location for the shift.");

                validationErrors.AddRange(sheriff!.AwayLocation.Select(aw => PrintSheriffEventConflict<SheriffAwayLocation>(aw.Sheriff, aw.StartDate, aw.EndDate)));
                validationErrors.AddRange(sheriff.Leave.Select(aw => PrintSheriffEventConflict<SheriffLeave>(aw.Sheriff, aw.StartDate, aw.EndDate)));
                validationErrors.AddRange(sheriff.Training.Select(aw => PrintSheriffEventConflict<SheriffTraining>(aw.Sheriff, aw.StartDate, aw.EndDate)));
            }

            if (validationErrors.Any())
                throw new BusinessLayerException(validationErrors.ListToStringWithPipes());
        }

        private async Task CheckForShiftOverlap(List<Shift> shifts)
        {
            shifts.ThrowBusinessExceptionIfEmpty("No shifts were provided.");
            if (shifts.Any(a =>
                shifts.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate && a.SheriffId == b.SheriffId)))
                throw new BusinessLayerException("Shifts provided overlap with themselves.");

            var overlappingShifts = await OverlappingShifts(shifts.First().LocationId, shifts);
            if (overlappingShifts.Any())
            {
                var message = overlappingShifts.Select(ol => ConflictingSheriffAndSchedule(ol.Sheriff, ol)).ToList()
                    .ListToStringWithPipes();
                throw new BusinessLayerException(message);
            }
        }
    
        private async Task<List<Shift>> OverlappingShifts(int locationId, List<Shift> targetShifts)
        {
            var dateMin = targetShifts.Min(ts => ts.StartDate);
            var dateMax = targetShifts.Max(ts => ts.EndDate);
            var sheriffIds = targetShifts.Select(ts => ts.SheriffId).Distinct();

            var shifts = await Db.Shift.AsNoTracking()
                .Include(s=> s.Sheriff)
                .Where(s =>
                    s.ExpiryDate == null &&
                    s.LocationId == locationId &&
                    s.StartDate >= dateMin &&
                    s.EndDate <= dateMax &&
                    sheriffIds.Contains(s.SheriffId)
                ).ToListAsync();

            return shifts.WhereToList(s =>
                targetShifts.Any(ts =>
                    ts.ExpiryDate == null && s.Id != ts.Id && ts.StartDate < s.EndDate && s.StartDate < ts.EndDate)
            );
        }
        #endregion Availability

        #region String Helpers
        private static string ConflictingSheriffAndSchedule(Sheriff sheriff, Shift shift)
            => $"Conflict - {nameof(Sheriff)}: {sheriff.LastName}, {sheriff.FirstName} - Existing Shift conflicts: {shift.StartDate.ConvertToTimezone(shift.Timezone)} -> {shift.EndDate.ConvertToTimezone(shift.Timezone)}";

        private static string PrintSheriffEventConflict<T>(Sheriff sheriff, DateTimeOffset start, DateTimeOffset end)
            => $"{sheriff.LastName}, {sheriff.FirstName} has an event {typeof(T).Name.Replace("Sheriff", "").ConvertCamelCaseToMultiWord()} from: {start} to: {end}";
        #endregion String Helpers

        #endregion
    }
}
