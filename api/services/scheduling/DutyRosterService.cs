using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.Api.services.scheduling
{
    public class DutyRosterService
    {
        private SheriffDbContext Db { get; }
        public DutyRosterService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<Duty>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end) =>
            await Db.Duty.AsNoTracking().Include(d => d.DutySlots)
                .Include(d=> d.Assignment)
                .Include(d=> d.Location)
                .Where(d => d.LocationId == locationId &&
                            d.StartDate < end &&
                            start < d.EndDate &&
                            d.ExpiryDate == null)
                .ToListAsync();

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
        ///  This can be used to assign slots.
        /// </summary>
        public async Task<List<Duty>> UpdateDuties(List<Duty> duties, bool overrideValidation)
        {
            var dutyIds = duties.SelectToList(s => s.Id);
            var savedDuties = Db.Duty.Where(s => dutyIds.Contains(s.Id));

            foreach (var duty in duties)
            {
                var savedDuty = savedDuties.FirstOrDefault(s => s.Id == duty.Id);
                duty.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
                savedDuty.ThrowBusinessExceptionIfNull($"{nameof(Duty)} with the id: {duty.Id} could not be found. ");
                
                foreach (var dutySlots in duty.DutySlots)
                {
                    if (!overrideValidation)
                    {
                        //TODO Check for assigned shift conflicts, if adjusting the duty start / end.  - For example a duty being extended past a Sheriff's schedule.
                        //Check if this exists. If it doesn't have an ID, create it. 
                    }




                }
                Db.Entry(savedDuty!).CurrentValues.SetValues(duty);
                savedDuty.DutySlots = duty.DutySlots;
            }
            await Db.SaveChangesAsync();
            return await savedDuties.ToListAsync();
        }

        public async Task ExpireDuty(int id)
        {
            await Db.DutySlot.Where(ds => ds.DutyId == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.Duty.Where(d => d.Id == id).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }
    }
}
