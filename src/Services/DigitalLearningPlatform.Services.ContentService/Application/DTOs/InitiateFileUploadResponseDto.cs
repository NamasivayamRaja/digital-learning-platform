namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public class InitiateFileUploadResponseDto
    {
        public Guid SectionFileId { get; set; }
        public Guid UploadId { get; set; }
        public string UploadUrl { get; set; } = default!;
    }
}
