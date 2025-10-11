using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.Application.Interfaces
{
    public interface ICourseContentService
    {
        Task<PagedList<CourseSummaryDto>> ListCoursesAsync(CourseParam courseParam);
        Task<CourseDetailDto?> GetCourseDetailAsync(Guid courseId);
    }
}
