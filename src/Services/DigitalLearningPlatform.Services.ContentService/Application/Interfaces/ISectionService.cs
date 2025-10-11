using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.Interfaces
{
    public interface ISectionService
    {
        Task<CreateSectionResponseDto> CreateDraftSectionAsync(Guid authorId, Guid courseId);
        Task UpdateSectionAsync(Guid authorId, UpdateSectionRequestDto saveSectionDto);
        Task DeleteSectionAsync(Guid authorId, Guid sectionId);
        Task<IEnumerable<SectionDto>> GetSectionByCourseIdAndAuthorAsync(Guid authorId, Guid courseId);
        Task<IEnumerable<SectionFileDto>> GetSectionDetailAsync(Guid sectionId, Guid authorId);
        Task<Guid> CreateSectionFileAsync(InitiateFileUploadRequestDto dto, Guid authorId);
    }
}
