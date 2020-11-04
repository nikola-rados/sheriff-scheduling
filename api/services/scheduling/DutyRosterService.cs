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
        private ShiftService ShiftService { get; }
        public DutyRosterService(SheriffDbContext db, ShiftService shiftService)
        {
            Db = db;
            ShiftService = shiftService;
        }

        public async Task<List<Duty>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end) =>
            await Db.Duty.AsNoTracking().Include(d => d.Shift)
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
                duty.Timezone.ThrowBusinessExceptionIfNullOrEmpty($"{nameof(duty.Timezone)} cannot be null or empty.");
                duty.Location = await Db.Location.FindAsync(duty.LocationId);
                duty.Assignment = await Db.Assignment.FindAsync(duty.AssignmentId);
                duty.Shift = await Db.Shift.FindAsync(duty.ShiftId);
                await Db.Duty.AddAsync(duty);
            }
            await Db.SaveChangesAsync();
            return duties;
        }

        public async Task<List<Duty>> UpdateDuties(List<Duty> duties)
        {
            foreach (var duty in duties)
            {
                duty.Timezone.ThrowBusinessExceptionIfNullOrEmpty($"{nameof(duty.Timezone)} cannot be null or empty.");
                var savedDuty = await Db.Duty.FindAsync(duty.Id);
                savedDuty.ThrowBusinessExceptionIfNull($"{nameof(Duty)} with the id: {duty.Id} could not be found. ");

                if (await Db.Shift.AsNoTracking().AnyAsync(d => d.Id == duty.ShiftId))
                    await AdjustShiftStartEndForDuty(duty, duty.ShiftId);

                Db.Entry(savedDuty).CurrentValues.SetValues(duty);
            }
            await Db.SaveChangesAsync();
            return duties;
        }

        /// <summary>
        /// This is for drag and drop functionality. 
        /// </summary>
        public async Task<Duty> AssignDuty(int id, int? shiftId)
        {
            var duty = await Db.Duty.FindAsync(id);

            if (await Db.Shift.AsNoTracking().AnyAsync(d => d.Id == duty.ShiftId))
                await AdjustShiftStartEndForDuty(duty, shiftId!.Value);

            //todo.
            await Db.SaveChangesAsync();
            return duty;
        }

        public async Task ExpireDuties(List<int> ids)
        {
            await Db.Duty.Where(d => ids.Contains(d.Id)).ForEachAsync(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            await Db.SaveChangesAsync();
        }

        private async Task AdjustShiftStartEndForDuty(Duty duty, int shiftId)
        {
            DateTimeOffset? start = null;
            DateTimeOffset? end = null;

            //todo.
            //Calculate the beginning or end adjustment. 

            await ShiftService.AdjustShift(shiftId, start, end);
        }
    }
}
