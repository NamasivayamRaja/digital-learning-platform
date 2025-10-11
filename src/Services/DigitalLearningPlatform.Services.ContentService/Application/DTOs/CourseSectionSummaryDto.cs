namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public class CourseSectionSummaryDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = "";
        public List<SectionDto> Sections { get; set; } = new();

    }
}
