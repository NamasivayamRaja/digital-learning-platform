using DigitalLearningPlatform.BuildingBlocks.Common.Utilities.Interfaces;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Utilities
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime TodayAsDateTime => DateTime.UtcNow.Date;
        public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    }
}
