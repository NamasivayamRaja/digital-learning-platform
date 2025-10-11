using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    /// <summary>
    /// Enterprise AuthorLookUp repository with idempotent operations
    /// Designed for event-driven scenarios with proper error handling
    /// </summary>
    public class AuthorLookUpRepository : IAuthorLookUpRepository
    {
        private readonly CourseDbContext _context;

        public AuthorLookUpRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorLookUp?> GetByAuthorIdAsync(Guid authorId)
        {
            return await _context.AuthorLookUps
                .FirstOrDefaultAsync(a => a.AuthorId == authorId);
        }

        public async Task<bool> ExistsAsync(Guid authorId)
        {
            return await _context.AuthorLookUps
                .AnyAsync(a => a.AuthorId == authorId);
        }

        public async Task AddAsync(AuthorLookUp authorLookUp)
        {
            await _context.AuthorLookUps.AddAsync(authorLookUp);
        }

        public async Task UpdateAsync(AuthorLookUp authorLookUp)
        {
            _context.AuthorLookUps.Update(authorLookUp);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid authorId)
        {
            var author = await GetByAuthorIdAsync(authorId);
            if (author != null)
            {
                _context.AuthorLookUps.Remove(author);
            }
        }

        public async Task<IEnumerable<AuthorLookUp>> GetAllAsync()
        {
            return await _context.AuthorLookUps.ToListAsync();
        }

        /// <summary>
        /// Idempotent upsert operation for event-driven scenarios
        /// Prevents duplicate entries when events are processed multiple times
        /// </summary>
        public async Task UpsertAsync(AuthorLookUp authorLookUp)
        {
            var existing = await GetByAuthorIdAsync(authorLookUp.AuthorId);
            
            if (existing != null)
            {
                // Update existing record
                existing.AuthorName = authorLookUp.AuthorName;
                _context.AuthorLookUps.Update(existing);
            }
            else
            {
                // Create new record
                await _context.AuthorLookUps.AddAsync(authorLookUp);
            }
        }
    }
}