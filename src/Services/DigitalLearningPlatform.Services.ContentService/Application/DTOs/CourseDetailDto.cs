namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record CourseDetailDto : CourseSummaryDto
    {
        public string Description { get; set; } = "";
        public List<SectionDetailDto> Sections { get; set; } = new();

    }
}
