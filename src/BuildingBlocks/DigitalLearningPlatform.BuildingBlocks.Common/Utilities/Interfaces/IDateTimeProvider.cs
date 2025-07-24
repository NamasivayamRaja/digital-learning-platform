namespace DigitalLearningPlatform.BuildingBlocks.Common.Utilities.Interfaces
{
    /// <summary>
    /// Abstraction for DateTime operations to improve testability
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime TodayAsDateTime { get; }
        DateOnly Today {  get; }
    }
}
