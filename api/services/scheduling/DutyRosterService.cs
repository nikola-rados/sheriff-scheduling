using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.infrastructure.exceptions;
using SS.Common.helpers.extensions;
using SS.Db.models.sheriff;

namespace SS.Api.services.scheduling
{
    public class DutyRosterService
    {
        private SheriffDbContext Db { get; }
        public DutyRosterService(SheriffDbContext db)
        {
            Db = db;
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

        /// <summary>
        /// This just creates, there is no assignment of slots. The team has agreed it's just created with the defaults initially.
        /// Creating a Duty with no slots, doesn't require any validation rules. 
        /// </summary>
        public async Task<Duty> AddDuty(Duty duty)
        {
            duty.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
            duty.Location = await Db.Location.FindAsync(duty.LocationId);
            duty.Assignment = await Db.Assignment.FindAsync(duty.AssignmentId);
            duty.DutySlots = new List<DutySlot>();
            await Db.Duty.AddAsync(duty);
            await Db.SaveChangesAsync();
            return duty;
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
                    if (savedDutySlot == null) 
                        await Db.DutySlot.AddAsync(dutySlot);
                    else 
                        Db.Entry(savedDutySlot).CurrentValues.SetValues(dutySlot);
                }
                Db.RemoveRange(savedDuty.DutySlots.Where(ds => duty.DutySlots.All(d => d.Id != ds.Id)));
            }

            /*var dutySlots = duties.SelectMany(d => d.DutySlots).ToList(); Overtime calculations, with shift adjustments. 
            var shiftIdsFromDutySlots = dutySlots.SelectDistinctToList(ds => ds.ShiftId);
            var shifts = Db.Shift.Where(s => shiftIdsFromDutySlots.Contains(s.Id) && s.ExpiryDate == null);
            var shiftDutySlotGroupBy = dutySlots.GroupBy(ds => ds.ShiftId).ToList();

            foreach (var shift in shifts)
            {
                shift.OriginalEndDate = shift.EndDate;
                shift.EndDate = shiftDutySlotGroupBy.First(k => k.Key == shift.Id).Max(ds => ds.EndDate);
            }*/

            await Db.SaveChangesAsync();

            return await savedDuties.ToListAsync();
        }

        public async Task ExpireDuty(int id)
        {
            await Db.DutySlot.Where(ds => ds.DutyId == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.Duty.Where(d => d.Id == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }

        #region Helpers

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

        #region String Helpers
        private static string ConflictingSheriffAndDutySlot(Sheriff sheriff, DutySlot dutySlot)
            => $"Conflict - {nameof(Sheriff)}: {sheriff?.LastName}, {sheriff?.FirstName} - Existing {nameof(DutySlot)} conflicts: {dutySlot.StartDate.ConvertToTimezone(dutySlot.Timezone)} -> {dutySlot.EndDate.ConvertToTimezone(dutySlot.Timezone)}";

        #endregion


        #endregion
    }
}
