using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record SectionDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public int Order { get; set; }
        public CreationStatus? Status { get; set; }
        public TimeSpan? VideoLength { get; set; }
        public List<SectionFileDto> Files { get; set; } = new();

    }
}
