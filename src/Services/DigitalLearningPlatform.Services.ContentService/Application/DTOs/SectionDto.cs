using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record SectionDto(
        Guid Id,
        string Title,
        int Order,
        CreationStatus Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
