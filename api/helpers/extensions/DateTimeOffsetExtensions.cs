using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;

namespace SS.Api.helpers.extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset TranslateDateIfDaylightSavings(this DateTimeOffset date, string timezone, int daysToShift)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];

            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            var movedZoned = zoned.Plus(Duration.FromDays(daysToShift));

            if (movedZoned.Offset != zoned.Offset)
                movedZoned = movedZoned.PlusHours(zoned.Offset.ToTimeSpan().Hours - movedZoned.Offset.ToTimeSpan().Hours);
            return movedZoned.ToDateTimeOffset();
        }

    }
}
