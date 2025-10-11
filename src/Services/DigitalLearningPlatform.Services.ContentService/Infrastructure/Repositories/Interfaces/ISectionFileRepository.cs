using DigitalLearningPlatform.Services.ContentService.Domain;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface ISectionFileRepository
    {
        Task<SectionFile?> GetByIdAsync(Guid id);
        Task<IEnumerable<SectionFile>> GetBySectionIdAsync(Guid sectionId);
        void Add(SectionFile file);
        void Update(SectionFile sectionFile);
        void AddRange(List<SectionFile> files);
        void Delete(SectionFile file);
        void DeleteRange(List<SectionFile> files);
    }
}
