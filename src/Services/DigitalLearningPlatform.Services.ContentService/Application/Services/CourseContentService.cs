using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;

namespace DigitalLearningPlatform.Services.ContentService.Application.Services
{
    public class CourseContentService : ICourseContentService
    {
        private readonly IContentQueryRepository _contentQueryRepository;
        public CourseContentService(IContentQueryRepository contentQueryRepository)
        {
            _contentQueryRepository = contentQueryRepository;
        }
        public Task<PagedList<CourseSummaryDto>> ListCoursesAsync(CourseParam courseParam)
        {
            return _contentQueryRepository.ListCoursesAsync(courseParam);
        }
        public Task<CourseDetailDto?> GetCourseDetailAsync(Guid courseId)
        {
            return _contentQueryRepository.GetCourseWithSectionsAndFilesAsync(courseId);
        }
    }
}
