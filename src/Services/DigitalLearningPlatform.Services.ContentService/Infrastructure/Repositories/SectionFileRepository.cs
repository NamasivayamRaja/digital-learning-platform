using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    public class SectionFileRepository : ISectionFileRepository
    {
        private readonly CourseDbContext _context;
        public SectionFileRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<SectionFile?> GetByIdAsync(Guid id)
        {
            return await _context.SectionFiles.FindAsync(id);
        }

        public async Task<IEnumerable<SectionFile>> GetBySectionIdAsync(Guid sectionId)
        {
            return await _context.SectionFiles
                .Where(x => x.SectionId == sectionId)
                .OrderBy(x=> x.Order)
                .ToListAsync();
        }

        public void Add(SectionFile file)
        {
            _context.SectionFiles.Add(file);
        }

        public void Delete(SectionFile file)
        {
            _context.SectionFiles.Remove(file);
        }
        // Efficient for up to 100 files upload if more file we need to use ef core bulk
        public void AddRange(List<SectionFile> files)
        {
            _context.SectionFiles.AddRange(files);
        }

        // Efficient for up to 100 files upload if more file we need use ef core bulk or raw sql query
        public void DeleteRange(List<SectionFile> files)
        {
            _context.SectionFiles.RemoveRange(files);
        }

        public void Update(SectionFile sectionFile)
        {
            _context.SectionFiles.Update(sectionFile);
        }
    }
}
