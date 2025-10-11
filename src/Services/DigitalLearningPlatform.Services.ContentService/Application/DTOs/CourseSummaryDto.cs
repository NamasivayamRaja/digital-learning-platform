namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record CourseSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Overview { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public string Category { get; set; } = "";
        public string Level { get; set; } = "";
        public TimeSpan TotalLength { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
