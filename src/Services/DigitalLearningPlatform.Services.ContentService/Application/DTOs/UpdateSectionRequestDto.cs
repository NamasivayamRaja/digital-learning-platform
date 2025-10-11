using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record UpdateSectionRequestDto
    {
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = default!;
        public CreationStatus Status { get; set; }
        public int Order { get; set; }                        
        public List<Guid>? CourseFileUploadIds { get; set; }
        public List<SectionFileSaveRequestDto>? CreateSectionFiles { get; set; }
    }
}
