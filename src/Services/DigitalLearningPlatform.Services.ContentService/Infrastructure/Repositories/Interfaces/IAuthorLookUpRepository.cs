using DigitalLearningPlatform.Services.ContentService.Domain;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for AuthorLookUp entity
    /// Follows enterprise patterns with async operations and idempotency support
    /// </summary>
    public interface IAuthorLookUpRepository
    {
        Task<AuthorLookUp?> GetByAuthorIdAsync(Guid authorId);
        Task<bool> ExistsAsync(Guid authorId);
        Task AddAsync(AuthorLookUp authorLookUp);
        Task UpdateAsync(AuthorLookUp authorLookUp);
        Task DeleteAsync(Guid authorId);
        Task<IEnumerable<AuthorLookUp>> GetAllAsync();
        
        /// <summary>
        /// Idempotent operation - creates or updates author lookup
        /// Essential for event-driven architectures where events might be processed multiple times
        /// </summary>
        Task UpsertAsync(AuthorLookUp authorLookUp);
    }
}