using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Controllers
{

    [Authorize]
    public class ContentQueryController : BaseController
    {
        private readonly ICourseContentService _contentService;

        public ContentQueryController(ICourseContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseSummaryDto>>> GetCourses([FromQuery] CourseParam courseParam)
        {
            var courses = await _contentService.ListCoursesAsync(courseParam);

            Response.AddPaginationResponseHeader(courses);

            return Ok(courses);
        }

        [HttpGet("{courseId}/details")]
        public async Task<ActionResult<CourseDetailDto>> GetCourseDetails(Guid courseId)
        {
            var courseDetails = await _contentService.GetCourseDetailAsync(courseId);

            if (courseDetails == null)
            {
                return NotFound();
            }

            return Ok(courseDetails);
        }
    }
}
