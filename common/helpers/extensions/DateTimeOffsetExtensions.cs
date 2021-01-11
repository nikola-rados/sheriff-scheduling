using System;
using NodaTime;
using TimeZoneNames;

namespace SS.Common.helpers.extensions
{
    public static class DateTimeExtensions
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

        public static DateTimeOffset TranslateDateForDaylightSavingsByHours(this DateTimeOffset date, string timezone, double hoursToShift)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];

            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            var movedZoned = zoned.Plus(Duration.FromHours(hoursToShift));

            if (movedZoned.Offset != zoned.Offset)
                movedZoned = movedZoned.PlusHours(zoned.Offset.ToTimeSpan().Hours - movedZoned.Offset.ToTimeSpan().Hours);
            return movedZoned.ToDateTimeOffset();
        }

        public static double HourDifference(this DateTimeOffset start, DateTimeOffset end, string timezone)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];
            var instantStart = Instant.FromDateTimeOffset(start);
            var instantEnd = Instant.FromDateTimeOffset(end);
            var zonedStart = instantStart.InZone(locationTimeZone);
            var zonedEnd = instantEnd.InZone(locationTimeZone);
            var duration =  ZonedDateTime.Subtract(zonedEnd, zonedStart);
            return duration.TotalHours;
        }

        public static DateTimeOffset DateOnly(this DateTimeOffset date) => new DateTimeOffset(date.Date, date.Offset);

        public static DateTimeOffset ConvertToTimezone(this DateTimeOffset date, string timezone)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];
            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            return zoned.ToDateTimeOffset();
        }

        private static string GetTimezoneAbbrevation(DateTimeOffset date, string timezone)
        {
            var abbreviations = TZNames.GetAbbreviationsForTimeZone(timezone, "en-CA");
            var abbreviation = date.IsDaylightSavings(timezone) ? abbreviations.Daylight : abbreviations.Standard;
            return abbreviation;
        }
        private static bool IsDaylightSavings(this DateTimeOffset date, string timezone)
        {
            var locationTimeZone = DateTimeZoneProviders.Tzdb[timezone];
            var instant = Instant.FromDateTimeOffset(date);
            var zoned = instant.InZone(locationTimeZone);
            return zoned.IsDaylightSavingTime();
        }

        public static string PrintFormatDateTime(this DateTimeOffset date, string timezone)
            => $"{date:ddd dd MMM yyyy HH:mm} {GetTimezoneAbbrevation(date, timezone)}";

        public static string PrintFormatDate(this DateTimeOffset date) => date.ToString("ddd dd MMM yyyy");
        public static string PrintFormatTime(this DateTimeOffset date, string timezone)
            => $"{date:HH:mm} {GetTimezoneAbbrevation(date, timezone)}";
    }
}
