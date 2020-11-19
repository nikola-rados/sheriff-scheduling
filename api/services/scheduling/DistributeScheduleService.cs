using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.models;
using SS.Db.models;
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

        public async Task<List<DistributeSchedule>> GetDistributeSchedule(List<ShiftAvailability> shiftAvailability, bool includeWorkSection)
        {
            foreach (var sa in shiftAvailability)
            {

            }

            //Combine duties, earliest sets the type. 
            var workSection = GetWorkSectionFromDuties();
            //var shifts = CombineShifts();

            /*var sheriffByDatesAndId =
                shifts.GroupBy(s => new {s.StartDate.ConvertToTimezone(s.Timezone).Date, s.SheriffId});

            //includeWorkSection
            WIP
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


            }*/
            return new List<DistributeSchedule>();
        }

        private string GetWorkSectionFromDuties()
        {
            return "";
        }

        private void CombineShifts()
        {

        }
    }
}
