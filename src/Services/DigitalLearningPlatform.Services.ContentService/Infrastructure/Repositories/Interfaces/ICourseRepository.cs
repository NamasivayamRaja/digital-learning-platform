using DigitalLearningPlatform.Services.ContentService.Domain;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid id);
        Task<Course?> GetByIdWithAuthorizationAsync(Guid courseId, Guid authorId);
        Task<IEnumerable<Course>> GetCoursesByAuthor(Guid authorId);
        void Add(Course course);
        void Update(Course course);
        void Delete(Course course);
        Task<bool> ExistsWithTitleAsync(string title);
        Task<bool> ExistsWithTitleAsync(string title, Guid courseId);
        Task<bool> IsAuthorHasPermission(Guid courseId, Guid authorId);
    }
}
