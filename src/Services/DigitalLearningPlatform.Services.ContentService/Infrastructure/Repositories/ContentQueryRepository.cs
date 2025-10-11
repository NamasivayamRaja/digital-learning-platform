using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Extensions;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.ContentService.Repositories
{
    public class ContentQueryRepository : IContentQueryRepository
    {
        private readonly CourseDbContext _context;
        public ContentQueryRepository(CourseDbContext context)
        {
            _context = context;
        }
        public async Task<PagedList<CourseSummaryDto>> ListCoursesAsync(CourseParam courseParam)
        {
            var query = _context.Courses
                .Where(c => c.Status ==  CreationStatus.Approved)
                .OrderByDescending(c => c.LastModifiedAt ?? c.CreatedAt).AsQueryable();

            if(!string.IsNullOrWhiteSpace(courseParam.Title))
                query = query.Where(x=> EF.Functions.ILike(x.Title, $"%{courseParam.Title}%"));

            if(courseParam.Category.HasValue)
                query = query.Where(x=> x.Category == courseParam.Category.Value);

            var projectedQuery = query.Select(c => new CourseSummaryDto
            {
                Id = c.Id,
                Title = c.Title,
                Overview = c.Overview,
                AuthorName = c.Author != null ? c.Author.AuthorName : "Unknown",
                Category = c.Category.ToString(),
                Level = c.Level.ToString(),
                TotalLength = c.TotalLength,
                LastUpdated = c.LastModifiedAt ?? c.CreatedAt
            });

            var items = await _context.Courses
    .Where(c => c.Status == CreationStatus.Approved)
                    .OrderByDescending(c => c.LastModifiedAt ?? c.CreatedAt)
    .ToListAsync(); // Confirm this returns 1

            var projected = items.Select(c => new CourseSummaryDto
            {
                Id = c.Id,
                Title = c.Title,
                AuthorName = c.Author != null ? c.Author.AuthorName : "Unknown"
            }).ToList(); // Should return 1


            return await projectedQuery.ToPagedList<CourseSummaryDto>(courseParam.PageNumber, courseParam.PageSize);
        }
        public async Task<CourseDetailDto?> GetCourseWithSectionsAndFilesAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Files)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.Status == CreationStatus.Approved);

            if (course == null)
                return null;

            var dto = new CourseDetailDto
            {
                Id = course.Id,
                Title = course.Title,
                Overview = course.Overview,
                AuthorName = course.Author != null ? course.Author.AuthorName : "Unknown",
                Category = course.Category.ToString(),
                Level = course.Level.ToString(),
                TotalLength = course.TotalLength,
                LastUpdated = course.LastModifiedAt ?? course.CreatedAt,
                Description = course.Description,
                Sections = course.Sections
                    .OrderBy(s => s.Order)
                    .Select(s => new SectionDetailDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Order = s.Order,
                        VideoLength = s.VideoLength,
                        Files = s.Files.Select(f => new SectionFileDto
                        {
                            Id = f.Id,
                            Title = f.Title,
                            FileName = f.FileName,
                            ContentType = f.ContentType,
                            BlobPath = f.BlobPath,
                            Size = f.Size
                        }).ToList()
                    }).ToList()
            };
            return dto;
        }
    }
}