using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces
{
    public interface IContentQueryRepository
    {
        // Returns paginated, filtered course summaries (as needed by UI)
        Task<PagedList<CourseSummaryDto>> ListCoursesAsync(CourseParam courseParam);
        // Returns a detailed course aggregate (course, sections, files, etc.)
        Task<CourseDetailDto?> GetCourseWithSectionsAndFilesAsync(Guid courseId);

    }
}
