using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {

        private readonly CourseDbContext _context;
        public CourseRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course?> GetByIdWithAuthorizationAsync(Guid courseId, Guid authorId)
        {
            return await _context.Courses
                .Where(c => c.Id == courseId && c.AuthorId == authorId)
                .FirstOrDefaultAsync();
        }

        public void Add(Course course)
        {
            _context.Add(course);
        }

        public void Update(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
        }

        public void Delete(Course course)
        {
            _context.Remove(course);
        }

        public async Task<bool> ExistsWithTitleAsync(string title)
        {
            return await _context.Courses.AnyAsync(c => EF.Functions.ILike(c.Title, title));
        }

        public async Task<bool> ExistsWithTitleAsync(string title, Guid courseId)
        {
            return await _context.Courses.AnyAsync(c => EF.Functions.ILike(c.Title, title) && c.Id != courseId);
        }

        public async Task<bool> IsAuthorHasPermission(Guid courseId, Guid authorId)
        {
            return await _context.Courses.AnyAsync(x => x.Id == courseId && x.AuthorId == authorId);
        }

        public async Task<IEnumerable<Course>> GetCoursesByAuthor(Guid authorId)
        {
            return await _context.Courses.Where(x => x.AuthorId == authorId).ToListAsync();
        }

    }
}
