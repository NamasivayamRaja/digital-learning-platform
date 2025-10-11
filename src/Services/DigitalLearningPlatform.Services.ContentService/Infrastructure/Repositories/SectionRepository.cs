using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly CourseDbContext _context;
        public SectionRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Section?> GetByIdAsync(Guid id)
        {
           return await _context.Sections.FindAsync(id);
        }

        public async Task<Section?> GetByIdWithAuthorizationAsync(Guid sectionId, Guid authorId)
        {
            return await _context.Sections
                .Include(s => s.Course)
                .Where(s => s.Id == sectionId && s.Course.AuthorId == authorId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Section>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Sections
                .Where(x => x.CourseId == courseId)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }

        public void Add(Section section)
        {
             _context.Sections.Add(section);   
        }

        public void Update(Section section)
        {
            _context.Sections.Update(section);
        }

        public void Delete(Section section)
        {
            _context.Sections.Remove(section);
        }

        public async Task<int> GetSectionCountByCourseIdAsync(Guid courseId, Guid authorId)
        {
            return await _context.Sections
                .Join(_context.Courses, s => s.CourseId, c => c.Id, (s, c) => new { s, c })
                .Where(x => x.s.CourseId == courseId && x.c.AuthorId == authorId)
                .CountAsync();
        }

        public async Task<IEnumerable<SectionDto>> GetSectionsByCourseIdAndAuthorAsync(Guid courseId, Guid authorId)
        {
            return await _context.Sections.Include(s=> s.Course)
                .Where(s => s.CourseId == courseId &&
                            s.Course.AuthorId == authorId) // Ensures author match
                .OrderBy(s => s.Order)
                .Select(s => new SectionDto(
                    s.Id,
                    s.Title,
                    s.Order,
                    s.Status,
                    s.CreatedAt,
                    s.LastModifiedAt
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<SectionFileDto>> GetSectionDetailAsync(Guid sectionId, Guid authorId)
        {
            return await _context.SectionFiles
                .Include(sf => sf.Section)
                .Where(sf => sf.Section.Id == sectionId &&
                            sf.Section.Course.AuthorId == authorId)
                .OrderBy(sf => sf.Order)
                .Select(sf => new SectionFileDto
                {
                    Id = sf.Id,
                    Title = sf.Title,
                    FileName = sf.FileName,
                    ContentType = sf.ContentType,
                    Size = sf.Size,
                    BlobPath = sf.BlobPath,
                    Order = sf.Order,
                    Status = sf.Status
                })
                .ToListAsync();
        }
    }
}
