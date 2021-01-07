using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SS.Api.helpers;
using SS.Api.infrastructure.exceptions;
using SS.Common.helpers.extensions;
using SS.Db.models.sheriff;
using SS.Api.models;

namespace SS.Api.services.scheduling
{
    public class DutyRosterService
    {
        private SheriffDbContext Db { get; }
        private ShiftService ShiftService { get; }
        private double OvertimeHoursPerDay { get; }
        private ILogger<DutyRosterService> Logger { get; }
        public DutyRosterService(SheriffDbContext db, IConfiguration configuration, ShiftService shiftService, ILogger<DutyRosterService> logger)
        {
            Db = db;
            ShiftService = shiftService;
            OvertimeHoursPerDay = double.Parse(configuration.GetNonEmptyValue("OvertimeHoursPerDay"));
            Logger = logger;
        }

        public async Task<List<Duty>> GetDutiesForLocation(int locationId, DateTimeOffset start, DateTimeOffset end) =>
            await Db.Duty.AsSingleQuery().AsNoTracking()
                .Include(d => d.DutySlots.Where(ds => ds.ExpiryDate == null))
                .Include(d=> d.Location)
                .Include(d => d.Assignment)
                .ThenInclude(d => d.LookupCode)
                .Where(d => d.LocationId == locationId &&
                            d.StartDate < end &&
                            start < d.EndDate &&
                            d.ExpiryDate == null)
                .ToListAsync();


        public async Task<List<int>> GetDutiesLocations(List<int> ids) =>
            await Db.Duty.AsNoTracking().In(ids, d => d.Id).Select(d => d.LocationId).Distinct().ToListAsync();

        public async Task<Duty> GetDutyByDutySlot(int dutySlotId) =>
            await Db.Duty.AsNoTracking().FirstOrDefaultAsync(d => d.DutySlots.Any(ds => ds.Id == dutySlotId));

        /// <summary>
        /// This just creates, there is no assignment of slots. The team has agreed it's just created with the defaults initially.
        /// Creating a Duty with no slots, doesn't require any validation rules. 
        /// </summary>
        public async Task<List<Duty>> AddDuties(List<Duty> duties)
        {
            foreach (var duty in duties)
            {
                duty.Timezone.GetTimezone()
                    .ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
                duty.Location = await Db.Location.FindAsync(duty.LocationId);
                duty.Assignment = await Db.Assignment.FindAsync(duty.AssignmentId);
                duty.DutySlots = new List<DutySlot>();
                await Db.Duty.AddAsync(duty);
            }

            await Db.SaveChangesAsync();
            return duties;
        }

        /// <summary>
        ///  This can be used to add / update / remove slots. 
        /// </summary>
        public async Task<List<Duty>> UpdateDuties(List<Duty> duties, bool overrideValidation)
        {
            if (!duties.Any()) throw new BusinessLayerException("Didn't provide any duties.");

            await CheckForDutySlotOverlap(duties, overrideValidation);
            
            var dutyIds = duties.SelectToList(duty => duty.Id);
            var savedDuties = Db.Duty.AsSingleQuery()
                .Include(d => d.DutySlots)
                .In(dutyIds, s => s.Id)
                .Where(s => s.ExpiryDate == null);

            foreach (var duty in duties)
            {
                var savedDuty = await savedDuties.FirstOrDefaultAsync(s => s.Id == duty.Id);
                savedDuty.ThrowBusinessExceptionIfNull($"{nameof(Duty)} with the id: {duty.Id} could not be found. ");
                duty.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
                Db.Entry(savedDuty!).CurrentValues.SetValues(duty);
                Db.Entry(savedDuty).Property(x => x.LocationId).IsModified = false;
                Db.Entry(savedDuty).Property(x => x.ExpiryDate).IsModified = false;

                foreach (var dutySlot in duty.DutySlots)
                {
                    var savedDutySlot = await Db.DutySlot
                        .FirstOrDefaultAsync(ds => ds.Id == dutySlot.Id && ds.ExpiryDate == null);
                    dutySlot.LocationId = duty.LocationId;
                    dutySlot.DutyId = duty.Id;
                    dutySlot.Timezone = duty.Timezone;
                    dutySlot.Duty = null;
                    dutySlot.Sheriff = null;
                    dutySlot.Location = null;
                    
                    if (savedDutySlot == null)
                        await Db.DutySlot.AddAsync(dutySlot);
                    else
                        Db.Entry(savedDutySlot).CurrentValues.SetValues(dutySlot);
                }
                Db.RemoveRange(savedDuty.DutySlots.Where(ds => duty.DutySlots.All(d => d.Id != ds.Id)));
            }

            await Db.SaveChangesAsync();

            var sheriffsForDuties = duties.SelectMany(d => d.DutySlots).SelectDistinctToList(ds => ds.SheriffId);
            var firstDuty = duties.First();
            await HandleShiftAdjustmentsAndOvertime(firstDuty.LocationId, firstDuty.StartDate, firstDuty.Timezone, sheriffsForDuties);
            await Db.SaveChangesAsync();

            return await savedDuties.ToListAsync();
        }

        public async Task<Duty> MoveSheriffFromDutySlot(int fromDutySlotId, int toDutyId, DateTimeOffset? separationTime = null)
        {
            var fromDutySlot = await Db.DutySlot.FirstOrDefaultAsync(d => d.Id == fromDutySlotId);
            fromDutySlot.ThrowBusinessExceptionIfNull("From duty slot didn't exist.");

            var fromSheriffId = fromDutySlot.SheriffId;
            if (!fromSheriffId.HasValue)
                throw new BusinessLayerException("No Sheriff Id provided.");

            var toDutySlotStart = separationTime ?? DateTimeOffset.UtcNow.ConvertToTimezone(fromDutySlot.Timezone);
            toDutySlotStart = RoundUp(toDutySlotStart, TimeSpan.FromMinutes(15));

            fromDutySlot.EndDate = toDutySlotStart;

            if (fromDutySlot.StartDate == fromDutySlot.EndDate)
                Db.DutySlot.Remove(fromDutySlot);

            var toDuty = await Db.Duty.Include(d=> d.DutySlots).FirstOrDefaultAsync(d => d.Id == toDutyId);
            if (!(toDutySlotStart < toDuty.EndDate && toDuty.StartDate < toDutySlotStart))
                throw new BusinessLayerException("Duty doesn't have room. You may need to adjust the duty.");

            var toDutySlotEnd = toDuty.EndDate;

            //Might be cut even shorter from existing dutyslots with sheriffs in them. 
            if (toDuty.DutySlots.Any(ds =>
                toDutySlotStart < ds.EndDate && ds.StartDate < toDutySlotEnd && ds.SheriffId != null))
            {
                var earliestEndFromOtherSlots = toDuty.DutySlots.Where(ds =>
                        toDutySlotStart < ds.EndDate && ds.StartDate < toDutySlotEnd && ds.SheriffId != null)
                    .Min(ds => ds.StartDate);

                if (earliestEndFromOtherSlots <= toDutySlotStart)
                    throw new BusinessLayerException("This already has a duty slot with a sheriff in it.");

                toDutySlotEnd = toDutySlotEnd > earliestEndFromOtherSlots ? earliestEndFromOtherSlots : toDutySlotEnd;
            }

            var maxShiftEndDate = await FindContinuousEndDateOverShifts(toDuty.LocationId, fromSheriffId.Value, toDutySlotStart, toDuty.Timezone);
            toDutySlotEnd = toDutySlotEnd > maxShiftEndDate ? maxShiftEndDate : toDutySlotEnd;

            var toDutySlot = toDuty.DutySlots.FirstOrDefault(ds =>
                toDutySlotStart < ds.EndDate && ds.StartDate < toDutySlotEnd && ds.SheriffId == null);

            if (toDutySlot == null)
            {
                toDutySlot = new DutySlot
                {
                    LocationId = toDuty.LocationId,
                    DutyId = toDuty.Id,
                    Timezone = toDuty.Timezone,
                    StartDate = toDutySlotStart,
                    EndDate = toDutySlotEnd,
                    SheriffId = fromSheriffId
                };
                await Db.DutySlot.AddAsync(toDutySlot);
            }
            else
            {
                toDutySlot.StartDate = toDutySlotStart;
                toDutySlot.EndDate = toDutySlotEnd;
                toDutySlot.SheriffId = fromSheriffId;
            }

            await Db.SaveChangesAsync();

            return toDuty;
        }

        public async Task ExpireDuties(List<int> ids)
        {
            await Db.DutySlot.In(ids, ds => ds.DutyId).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.Duty.In(ids, d => d.Id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateDutyComment(int dutyId, string comment)
        {
            var savedDuty = await Db.Duty.FirstOrDefaultAsync(d => d.Id == dutyId);
            savedDuty.Comment = comment;
            await Db.SaveChangesAsync();
        }

        #region Helpers

        private async Task HandleShiftAdjustmentsAndOvertime(int locationId, DateTimeOffset targetDate, string timezone, IEnumerable<Guid?> sheriffsForDuties)
        {
            var startRange = targetDate.ConvertToTimezone(timezone).DateOnly();
            var endRange = startRange.TranslateDateForDaylightSavingsByHours(timezone, 24);

            var shiftExpansions = new List<ShiftExpansion>();
            foreach (var sheriff in sheriffsForDuties.Where(sheriff => sheriff != null))
            {
                var shiftsForDay = await Db.Shift.Where(s =>
                    s.ExpiryDate == null && s.SheriffId == sheriff && s.LocationId == locationId && s.StartDate >= startRange && s.StartDate < endRange).ToListAsync();

                if (shiftsForDay.Count == 0) continue;
                var referenceShift = shiftsForDay.First();

                var earliestShift = shiftsForDay.FirstOrDefault(s => s.StartDate == shiftsForDay.Min(s2 => s2.StartDate));
                var latestShift = shiftsForDay.FirstOrDefault(s => s.EndDate == shiftsForDay.Max(s2 => s2.EndDate));

                var dutySlotsForSheriffOnDay = await Db.DutySlot.Where(s =>
                    s.ExpiryDate == null && s.SheriffId == sheriff && s.LocationId == locationId && s.StartDate >= startRange && s.StartDate < endRange).ToListAsync();

                var earliestDutySlot = dutySlotsForSheriffOnDay.FirstOrDefault(ds => ds.StartDate == dutySlotsForSheriffOnDay.Min(ds2 => ds2.StartDate));
                var latestDutySlot = dutySlotsForSheriffOnDay.FirstOrDefault(ds => ds.EndDate == dutySlotsForSheriffOnDay.Max(ds2 => ds2.EndDate));

    
                if (latestShift!.EndDate < latestDutySlot?.EndDate)
                {
                    await Db.Shift.AddAsync(new Shift
                    {
                        SheriffId = sheriff.Value,
                        Timezone = referenceShift.Timezone,
                        LocationId = referenceShift.LocationId,
                        StartDate = latestShift.EndDate,
                        EndDate = latestDutySlot.EndDate
                    });
                    shiftExpansions.Add(new ShiftExpansion { SheriffId = sheriff.Value, Date = earliestDutySlot.StartDate, Timezone = earliestShift.Timezone });
                }

                if (earliestShift!.StartDate > earliestDutySlot?.StartDate)
                {
                    await Db.Shift.AddAsync(new Shift
                    {
                        SheriffId = sheriff.Value,
                        Timezone = referenceShift.Timezone,
                        LocationId = referenceShift.LocationId,
                        StartDate = earliestDutySlot.StartDate,
                        EndDate = earliestShift.StartDate
                    });
                    shiftExpansions.Add(new ShiftExpansion { SheriffId = sheriff.Value, Date = earliestDutySlot.StartDate, Timezone = earliestShift.Timezone });
                }
            }

            if (!shiftExpansions.Any()) return;
            await Db.SaveChangesAsync();

            foreach (var shiftExpansion in shiftExpansions.Distinct())
            {
                await ShiftService.CalculateOvertimeHoursForSheriffOnDay(
                    shiftExpansion.SheriffId, shiftExpansion.Date,
                    shiftExpansion.Timezone);
            }
        }

        //There might be a way to make this more generic and reuse it. I think overlapping shifts and DutySlot overlapping is a very similar process. 
        public async Task CheckForDutySlotOverlap(List<Duty> duties, bool overrideValidation)
        {
            if (overrideValidation) return;

            var locationId = duties.First().LocationId;
            var overlappingDutySlots = await OverlappingDutySlots(locationId, duties);
            if (overlappingDutySlots.Any())
            {
                var message = overlappingDutySlots.Select(ol => ConflictingSheriffAndDutySlot(ol.Sheriff, ol)).ToList()
                    .ToStringWithPipes();
                throw new BusinessLayerException(message);
            }
        }

        private async Task<List<DutySlot>> OverlappingDutySlots(int locationId, List<Duty> duties)
        {
            var dutySlots = duties.SelectMany(s => s.DutySlots).ToList();
            if (dutySlots.Any(a =>
                dutySlots.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate && a.SheriffId == b.SheriffId)))
                throw new BusinessLayerException($"{nameof(DutySlot)} provided overlap with themselves.");

            var sheriffIds = dutySlots.Select(ts => ts.SheriffId).Where(sId => sId != null).Distinct().ToList();

            var conflictingDutySlots = new List<DutySlot>();
            foreach (var ts in dutySlots)
            {
                conflictingDutySlots.AddRange(await Db.DutySlot.AsNoTracking()
                    .Include(s => s.Sheriff)
                    .In(sheriffIds, ds => ds.SheriffId)
                    .Where(s =>
                        s.ExpiryDate == null &&
                        s.LocationId == locationId &&
                        s.StartDate < ts.EndDate && ts.StartDate < s.EndDate
                    ).ToListAsync());
            }

            conflictingDutySlots = conflictingDutySlots.Distinct().WhereToList(s =>
                dutySlots.Any(ts =>
                    ts.ExpiryDate == null && s.Id != ts.Id && ts.StartDate < s.EndDate && s.StartDate < ts.EndDate && ts.SheriffId == s.SheriffId) && 
                dutySlots.All(ts => ts.Id != s.Id)
            );

            return conflictingDutySlots;
        }

        private DateTimeOffset RoundUp(DateTimeOffset dt, TimeSpan d)
        {
            return new DateTimeOffset((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Offset);
        }

        private async Task<DateTimeOffset> FindContinuousEndDateOverShifts(int locationId, Guid sheriffId, DateTimeOffset targetDate, string timezone)
        {
            var startRange = targetDate;
            var endRange = startRange.DateOnly().TranslateDateForDaylightSavingsByHours(timezone, 24);
            
            var shifts = await Db.Shift.Where(s =>
                s.ExpiryDate == null && s.LocationId == locationId && s.SheriffId == sheriffId && s.EndDate >= startRange && s.StartDate < endRange).ToListAsync();

            Shift previousShift = null;
            DateTimeOffset endDate = default;
            foreach (var shift in shifts.OrderBy(s => s.StartDate))
            {
                if (previousShift != null && previousShift.EndDate != shift.StartDate)
                    break;
                previousShift = shift;
                endDate = shift.EndDate;
            }
            return endDate;
        }

        #region String Helpers
        private static string ConflictingSheriffAndDutySlot(Sheriff sheriff, DutySlot dutySlot)
            => $"Conflict - {nameof(Sheriff)}: {sheriff?.LastName}, {sheriff?.FirstName} - Existing {nameof(DutySlot)} conflicts: {dutySlot.StartDate.ConvertToTimezone(dutySlot.Timezone)} -> {dutySlot.EndDate.ConvertToTimezone(dutySlot.Timezone)}";

        #endregion


        #endregion
    }
}
