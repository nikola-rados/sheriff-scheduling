using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.models;
using SS.Common.helpers.extensions;
using SS.Db.models;

namespace SS.Api.services.scheduling
{
    public class DistributeScheduleService
    {
        private SheriffDbContext Db { get; }
        public DistributeScheduleService(SheriffDbContext db)
        {
            Db = db;
        }

        public async Task<List<DistributeSchedule>> GetDistributeSchedule(DateTimeOffset start, DateTimeOffset end, int locationId, bool includeWorkSection)
        {
            //Get the sheriffs that are loaned in / out.
            var sheriffs = Db.Sheriff.Where(s => s.IsEnabled &&
                                  s.HomeLocationId == locationId ||
                                  s.AwayLocation.Any(al =>
                                      al.LocationId == locationId && !(al.StartDate > end || start > al.EndDate)
                                                                  && al.ExpiryDate == null))
                .IncludeSheriffEventsBetweenDates(start,end);

            //Look at shifts by day, if same sheriff.. combine 
            //Get the shifts currently scheduled
            var shifts = await Db.Shift
                .Include(s => s.Duties).ThenInclude(d => d.DutySlots)
                .Where(s => s.LocationId == locationId && start < s.EndDate && s.StartDate < end && s.SheriffId != null)
                .ToListAsync();

            //Combine duties, earliest sets the type. 

            var sheriffByDatesAndId =
                shifts.GroupBy(s => new {s.StartDate.ConvertToTimezone(s.Timezone).Date, s.SheriffId});

            //includeWorkSection

            foreach (var sheriffByDateAndId in sheriffByDatesAndId)
            {
               // sh
            }

            foreach (var shift in shifts)
            {
                // var shiftStartDate = shiftDutySlots.Min(ds => ds.StartDate);
                //var shiftEndDate = shiftDutySlots.Max(ds => ds.EndDate);

                new DistributeSchedule
                {
                    Sheriff = sheriffs.FirstOrDefault(s => s.Id == shift.SheriffId)
                    //  StartDate = shiftStartDate,
                    //    EndDate = shiftEndDate
                };


            }
            return new List<DistributeSchedule>();
        }
    }
}
