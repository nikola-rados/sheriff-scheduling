using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Common.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using SS.Db.models.scheduling.notmapped;

namespace SS.Api.services.scheduling
{
    public class DistributeScheduleService
    {
        private SheriffDbContext Db { get; }
        public DistributeScheduleService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<ShiftAvailability>> GetDistributeSchedule(List<ShiftAvailability> shiftAvailabilities, bool includeWorkSection, DateTimeOffset start, DateTimeOffset end, int locationId)
        {
            var shiftIds = shiftAvailabilities.SelectMany(s=> s.Conflicts)
                .Where(c => c.Conflict == ShiftConflictType.Scheduled).SelectDistinctToList(s => s.ShiftId);

            var shifts = await Db.Shift.AsSingleQuery().AsNoTracking()
                .In(shiftIds, s => s.Id)
                .ToListAsync();

            var dutySlots = Db.DutySlot.AsSingleQuery()
                .AsNoTracking()
                .Include(ds => ds.Duty)
                .ThenInclude(d => d.Assignment)
                .ThenInclude(a => a.LookupCode)
                .Where(ds =>
                    ds.LocationId == locationId && ds.ExpiryDate == null && ds.StartDate < end && start < ds.EndDate);

            foreach (var shift in shifts)
                shift.DutySlots = dutySlots.Where(ds =>
                        ds.SheriffId == shift.SheriffId && ds.StartDate < shift.EndDate && shift.StartDate < ds.EndDate)
                    .ToList();

            foreach (var shiftAvailability in shiftAvailabilities)
                shiftAvailability.Conflicts = CombineShiftAvailability(shiftAvailability, shifts, includeWorkSection);

            return shiftAvailabilities;
        }

        private List<ShiftAvailabilityConflict> CombineShiftAvailability(ShiftAvailability shiftAvailability, List<Shift> shifts, bool includeWorkSection)
        {
            var shiftsGroupedByDate =
                shiftAvailability.Conflicts.Where(c => c.Conflict == ShiftConflictType.Scheduled)
                    .GroupBy(s => new
                    {
                        s.Start.ConvertToTimezone(s.Timezone)
                            .Date
                    });

            var newAvailabilityConflict = shiftAvailability.Conflicts.WhereToList(c => c.Conflict != ShiftConflictType.Scheduled);
            foreach (var group in shiftsGroupedByDate)
            {
                var earliestShiftForDate = group.First(s => s.Start == group.Min(s => s.Start));
                earliestShiftForDate.End = group.Max(s => s.End);
                newAvailabilityConflict.Add(earliestShiftForDate);
            }

            if (includeWorkSection)
                newAvailabilityConflict = DetermineWorkSections(newAvailabilityConflict, shifts);

            return newAvailabilityConflict;
        }

        private List<ShiftAvailabilityConflict> DetermineWorkSections(List<ShiftAvailabilityConflict> availabilityConflicts, List<Shift> shifts)
        {
            foreach (var availabilityConflict in availabilityConflicts)
                availabilityConflict.WorkSection =
                    shifts.FirstOrDefault(s => s.Id == availabilityConflict.ShiftId)?.WorkSection;

            return availabilityConflicts;
        }
    }
}
