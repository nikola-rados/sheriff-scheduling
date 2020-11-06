using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Db.models;
using SS.Db.models.scheduling;

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
            await Db.Duty.AsNoTracking().Include(d => d.Shifts)
                .Include(d=> d.Assignment)
                .Include(d=> d.Location)
                .Where(d => d.LocationId == locationId &&
                            d.StartDate < end &&
                            start < d.EndDate &&
                            d.ExpiryDate == null)
                .ToListAsync();

        public async Task<List<Duty>> AddDuties(List<Duty> duties)
        {
            foreach (var duty in duties)
            {
                duty.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
                duty.Location = await Db.Location.FindAsync(duty.LocationId);
                duty.Assignment = await Db.Assignment.FindAsync(duty.AssignmentId);
                duty.Shifts = new List<Shift>();
                await Db.Duty.AddAsync(duty);
            }
            await Db.SaveChangesAsync();
            return duties;
        }

        /// <summary>
        /// This will only allow changes on the base object, nothing complex. AssignDuty should handle assigning shifts.
        /// </summary>
        public async Task<List<Duty>> UpdateDuties(List<Duty> duties)
        {
            var dutyIds = duties.SelectToList(s => s.Id);
            var savedDuties = Db.Duty.Where(s => dutyIds.Contains(s.Id));

            foreach (var duty in duties)
            {
                var savedDuty = savedDuties.FirstOrDefault(s => s.Id == duty.Id);
                duty.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(duty.Timezone)} must be provided.");
                savedDuty.ThrowBusinessExceptionIfNull($"{nameof(Duty)} with the id: {duty.Id} could not be found. ");
                //TODO Check for assigned shift conflicts, if adjusting the duty start / end. 
                //For example a duty being extended past a Sheriff's schedule.
                Db.Entry(savedDuty!).CurrentValues.SetValues(duty);
            }
            await Db.SaveChangesAsync();
            return await savedDuties.ToListAsync();
        }

        /// <summary>
        /// This is for drag and drop functionality. 
        /// </summary>
        public async Task<Duty> AssignDuty(int id, int? shiftId)
        {
            var duty = await Db.Duty.FindAsync(id);
            var shift = await Db.Shift.FindAsync(shiftId);
            
            //Ensure no other duty for the shift is going on for this time. Otherwise remove?
            //Ensure the shift and duty have overlap.
            //TODO Adjust shift length?
            shift.Duties.Add(duty);
            duty.Shifts.Add(shift);

            await Db.SaveChangesAsync();
            return duty;
        }

        public async Task ExpireDuties(List<int> ids, string reason)
        {
            await Db.Duty.Where(d => ids.Contains(d.Id)).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }
    }
}
