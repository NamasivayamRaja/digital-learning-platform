using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.DTOs
{
    public record CreateSectionResponseDto(
    Guid Id,
    Guid CourseId,
    string Title,
    int Order,
    CreationStatus Status);

}
