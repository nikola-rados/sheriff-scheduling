using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using SS.Api.Models.DB;
using ss.db.models;
using SS.Db.models.lookupcodes;

namespace SS.Db.models.scheduling
{
    [AdaptTo("[name]Dto")]
    public class Assignment : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public LookupTypes AssignmentType { get; set; }
        public LookupCode LookupCode { get; set; } 
        public int? LookupCodeId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Timezone { get; set; }
        public string Name { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }

        public bool IsAvailableOnDate(DateTimeOffset utcDate)
        {
            return utcDate.UtcDateTime.DayOfWeek switch
            {
                DayOfWeek.Monday => Monday,
                DayOfWeek.Tuesday => Tuesday,
                DayOfWeek.Wednesday => Wednesday,
                DayOfWeek.Thursday => Thursday,
                DayOfWeek.Friday => Friday,
                DayOfWeek.Saturday => Saturday,
                DayOfWeek.Sunday => Sunday,
                _ => false
            };
        }

        public bool IsAvailableInRange(DateTimeOffset utcDateStart, DateTimeOffset utcDateEnd)
        {
            for (var dt = utcDateStart.UtcDateTime.Date; dt < utcDateEnd.UtcDateTime.Date; )
            {
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Monday when !Monday:
                    case DayOfWeek.Tuesday when !Tuesday:
                    case DayOfWeek.Wednesday when !Wednesday:
                    case DayOfWeek.Thursday when !Thursday:
                    case DayOfWeek.Friday when !Friday:
                    case DayOfWeek.Saturday when !Saturday:
                    case DayOfWeek.Sunday when !Sunday:
                        return false;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}
