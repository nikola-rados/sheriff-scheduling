using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SS.Api.helpers;
using SS.Api.infrastructure.exceptions;
using SS.Common.helpers.extensions;
using SS.Db.models.sheriff;
using ILogger = Castle.Core.Logging.ILogger;

namespace SS.Api.services.scheduling
{
    public class DutyRosterService
    {
        private SheriffDbContext Db { get; }
        private ShiftService ShiftService { get; }
        private double OvertimeHoursPerDay { get; }
        public DutyRosterService(SheriffDbContext db, IConfiguration configuration, ShiftService shiftService)
        {
            Db = db;
            ShiftService = shiftService;
            OvertimeHoursPerDay = double.Parse(configuration.GetNonEmptyValue("OvertimeHoursPerDay"));
        }

        public async Task<List<Duty>> GetDutiesForLocation(int locationId, DateTimeOffset start, DateTimeOffset end) =>
            await Db.Duty.AsSingleQuery().AsNoTracking()
                .Include(d => d.DutySlots.Where(ds => ds.ExpiryDate == null))
                .Include(d=> d.Assignment)
                .Include(d=> d.Location)
                .Where(d => d.LocationId == locationId &&
                            d.StartDate < end &&
                            start < d.EndDate &&
                            d.ExpiryDate == null)
                .ToListAsync();

        public async Task<List<int>> GetDutiesLocations(List<int> ids) =>
            await Db.Duty.AsNoTracking().Where(d => ids.Contains(d.Id)).Select(d => d.LocationId).Distinct().ToListAsync();

        public async Task<Duty> GetDuty(int id) =>
            await Db.Duty.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

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
                .Where(s => dutyIds.Contains(s.Id) && s.ExpiryDate == null);

            var shiftIds = duties
                .SelectMany(d => d.DutySlots.Select(ds => ds.ShiftId)).ToList()
                .Concat(savedDuties.SelectMany(d => d.DutySlots.Select(ds => ds.ShiftId)))
                .Distinct()
                .ToList();

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
                    dutySlot.Shift = null;
                    dutySlot.Location = null;
                    dutySlot.IsOvertime = false;
                    
                    if (savedDutySlot == null)
                        await Db.DutySlot.AddAsync(dutySlot);
                    else
                        Db.Entry(savedDutySlot).CurrentValues.SetValues(dutySlot);
                }
                Db.RemoveRange(savedDuty.DutySlots.Where(ds => duty.DutySlots.All(d => d.Id != ds.Id)));
            }

            await Db.SaveChangesAsync();

            await HandleShiftAdjustmentsAndOvertime(shiftIds);
            await Db.SaveChangesAsync();

            return await savedDuties.ToListAsync();
        }

        public async Task<Duty> MoveSheriffFromDutySlot(int fromDutySlotId, int toDutyId, DateTimeOffset? separationTime = null)
        {
            var fromDutySlot = await Db.DutySlot.FirstOrDefaultAsync(d => d.Id == fromDutySlotId);
            fromDutySlot.ThrowBusinessExceptionIfNull("From duty slot didn't exist.");

            var fromShiftId = fromDutySlot.ShiftId;
            var fromSheriffId = fromDutySlot.SheriffId;

            var toDutySlotStart = separationTime ?? DateTimeOffset.UtcNow.ConvertToTimezone(fromDutySlot.Timezone);
            toDutySlotStart = RoundUp(toDutySlotStart, TimeSpan.FromMinutes(15));

            var oldEndDate = fromDutySlot.EndDate;
            fromDutySlot.EndDate = toDutySlotStart;

            var toDuty = await Db.Duty.Include(d=> d.DutySlots).FirstOrDefaultAsync(d => d.Id == toDutyId);
            if (!(toDutySlotStart < toDuty.EndDate && toDuty.StartDate < toDutySlotStart))
                throw new BusinessLayerException("Duty doesn't have room. You may need to adjust the duty.");

            //Might be cut short from the new duty.
            var toDutySlotEnd = oldEndDate > toDuty.EndDate ? toDuty.EndDate : oldEndDate;

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

            fromDutySlot.ShiftId = null;
            fromDutySlot.SheriffId = null;

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
                    SheriffId = fromSheriffId,
                    ShiftId = fromShiftId
                };
                await Db.DutySlot.AddAsync(toDutySlot);
            }
            else
            {
                toDutySlot.StartDate = toDutySlotStart;
                toDutySlot.EndDate = toDutySlotEnd;
                toDutySlot.SheriffId = fromSheriffId;
                toDutySlot.ShiftId = fromShiftId;
            }

            await Db.SaveChangesAsync();

            return toDuty;
        }

        public async Task ExpireDuty(int id)
        {
            await Db.DutySlot.Where(ds => ds.DutyId == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.Duty.Where(d => d.Id == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }

        #region Helpers

        private async Task HandleShiftAdjustmentsAndOvertime(List<int?> shiftIds)
        {
            var shifts = await Db.Shift.Where(ds => shiftIds.Contains(ds.Id)).ToListAsync();
            foreach (var shift in shifts)
            {
                var dutySlotsForShift = Db.DutySlot.Where(ds => ds.ShiftId == shift.Id && ds.ExpiryDate == null);
                var earliestDutySlot = dutySlotsForShift.FirstOrDefault(ds => ds.StartDate == dutySlotsForShift.Min(ds2 => ds2.StartDate));
                var latestDutySlot = dutySlotsForShift.FirstOrDefault(ds => ds.EndDate == dutySlotsForShift.Max(ds2 => ds2.EndDate));

                var shiftExpandedByDutySlotIds = new List<int>();
                if (shift.EndDate < latestDutySlot?.EndDate)
                {
                    shift.EndDate = latestDutySlot.EndDate;
                    shiftExpandedByDutySlotIds.Add(latestDutySlot.Id);
                }

                if (shift.StartDate > earliestDutySlot?.StartDate)
                {
                    shift.StartDate = earliestDutySlot.StartDate;
                    shiftExpandedByDutySlotIds.Add(earliestDutySlot.Id);
                }

                if (shiftExpandedByDutySlotIds.Any())
                {
                    var overtimeHoursForSheriff = await ShiftService.CalculateOvertimeHoursForSheriffOnDay(
                        shift.SheriffId, shift.StartDate,
                        shift.Timezone);

                    if (earliestDutySlot != null && shiftExpandedByDutySlotIds.Contains(earliestDutySlot.Id)) 
                        earliestDutySlot.IsOvertime = overtimeHoursForSheriff > 0;
                    if (latestDutySlot != null && shiftExpandedByDutySlotIds.Contains(latestDutySlot.Id))  
                        latestDutySlot.IsOvertime = overtimeHoursForSheriff > 0;
                }
                else if (shift.StartDate.HourDifference(shift.EndDate, shift.Timezone) > OvertimeHoursPerDay)
                {
                    var regularShiftEndDate = shift.StartDate.TranslateDateForDaylightSavingsByHours(shift.Timezone, OvertimeHoursPerDay);
                    if (latestDutySlot?.EndDate < shift.EndDate)
                        shift.EndDate = latestDutySlot.EndDate < regularShiftEndDate ?  regularShiftEndDate : latestDutySlot.EndDate;

                    var regularShiftStartDate = shift.EndDate.TranslateDateForDaylightSavingsByHours(shift.Timezone, -OvertimeHoursPerDay);
                    if (earliestDutySlot?.StartDate > shift.StartDate)
                        shift.StartDate = earliestDutySlot.StartDate > regularShiftStartDate ? regularShiftStartDate : earliestDutySlot.StartDate;
                }
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

            var sheriffIds = dutySlots.Select(ts => ts.SheriffId).Where(sId => sId != null).Distinct();

            var conflictingDutySlots = new List<DutySlot>();
            foreach (var ts in dutySlots)
            {
                conflictingDutySlots.AddRange(await Db.DutySlot.AsNoTracking()
                    .Include(s => s.Sheriff)
                    .Where(s =>
                        s.ExpiryDate == null &&
                        s.LocationId == locationId &&
                        s.StartDate < ts.EndDate && ts.StartDate < s.EndDate &&
                        sheriffIds.Contains(s.SheriffId)
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

        #region String Helpers
        private static string ConflictingSheriffAndDutySlot(Sheriff sheriff, DutySlot dutySlot)
            => $"Conflict - {nameof(Sheriff)}: {sheriff?.LastName}, {sheriff?.FirstName} - Existing {nameof(DutySlot)} conflicts: {dutySlot.StartDate.ConvertToTimezone(dutySlot.Timezone)} -> {dutySlot.EndDate.ConvertToTimezone(dutySlot.Timezone)}";

        #endregion


        #endregion
    }
}
