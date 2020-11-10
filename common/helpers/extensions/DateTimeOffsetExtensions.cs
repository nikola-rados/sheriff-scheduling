using System;
using NodaTime;

namespace SS.Common.helpers.extensions
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

        public static DateTimeOffset ConvertToTimezone(this DateTimeOffset date, string timezone)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];
            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            return zoned.ToDateTimeOffset();
        }
    }
}
