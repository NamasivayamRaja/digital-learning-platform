using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface ISectionRepository
    {
        Task<Section?> GetByIdAsync(Guid id);
        Task<Section?> GetByIdWithAuthorizationAsync(Guid sectionId, Guid authorId);
        Task<IEnumerable<Section>> GetByCourseIdAsync(Guid courseId);
        void Add(Section section);
        void Update(Section section);
        void Delete(Section section);
        Task<int> GetSectionCountByCourseIdAsync(Guid courseId,Guid authorId);
        Task<IEnumerable<SectionDto>> GetSectionsByCourseIdAndAuthorAsync(Guid courseId, Guid authorId);
        Task<IEnumerable<SectionFileDto>> GetSectionDetailAsync(Guid sectionId, Guid authorId);
    }
}
