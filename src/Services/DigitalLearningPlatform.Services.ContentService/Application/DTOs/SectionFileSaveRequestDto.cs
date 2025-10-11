using System.ComponentModel.DataAnnotations;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record SectionFileSaveRequestDto
    {
        [Required]
        public Guid SectionId { get; set; }
        [Required]
        public Guid SectionFileId { get; set; }
        public string Title { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string BlobPath { get; set; } = default!; 
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }
        public int Order { get; set; }
        [Required]
        public Guid CourseFileUploadId { get; set; }
    }
}
