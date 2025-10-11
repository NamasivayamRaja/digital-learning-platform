using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record UpdateCourseRequestDto
    {
        public Guid Id { get; set; } 
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Overview { get; set; } = default!;
        public CourseCategory Category { get; set; }
        public CourseLevel Level { get; set; }
    }
}
