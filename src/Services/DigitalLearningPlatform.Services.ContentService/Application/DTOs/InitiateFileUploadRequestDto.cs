namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record InitiateFileUploadRequestDto
    {
        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        public Guid SectionId { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long FileSize { get; set; }
    }
}
