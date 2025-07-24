namespace DigitalLearningPlatform.BuildingBlocks.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EnsureUtc(this DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Utc 
                ? dateTime 
                : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0,DateTimeKind.Utc);
        }

        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23,59, 59, DateTimeKind.Utc);
        }

        public static string ToIsoDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToIsoDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static bool IsToday(this DateTime date) 
        {
            return date.Date == DateTime.UtcNow.Date;
        }
    }
}
