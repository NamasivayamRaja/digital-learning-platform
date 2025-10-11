using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories
{
    public class CourseFileUploadRepository(CourseDbContext context) : ICourseFileUploadRepository
    {
        public void Add(CourseFileUpload fileUpload)
        {
            context.CourseFileUploads.Add(fileUpload);
        }

        public async Task<CourseFileUpload?> GetByIdAsync(Guid id)
        {
            return await context.CourseFileUploads.FindAsync(id);
        }

        public async Task<IEnumerable<CourseFileUpload>> GetUploadsByCourseIdAsync(Guid courseId)
        {
            return await context.CourseFileUploads.Where(x => x.CourseId == courseId).ToListAsync();
        }

        public async Task<IEnumerable<CourseFileUpload>> GetUploadsByCourseIdAndStatusAsync(Guid courseId, FileProcessingStatus status)
        {
            return await context.CourseFileUploads
                .Where(x => x.CourseId == courseId && x.Status == status)
                .ToListAsync();
        }

        public void Update(CourseFileUpload fileUpload)
        {
            context.CourseFileUploads.Update(fileUpload);
        }
    }
}
