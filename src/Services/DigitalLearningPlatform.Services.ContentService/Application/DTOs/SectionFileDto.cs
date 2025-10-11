using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record SectionFileDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string BlobPath { get; set; } = string.Empty;
        public int Order { get; set; }
        public CreationStatus Status { get; set; }
    }
}
