using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Controllers
{
    [Authorize("Instructor")]    
    [Route("api/courses")]
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService) {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateCourseResponseDto>> Create([FromBody]CreateCourseRequestDto courseDto)
        {
            courseDto.AuthorId = User.GetUserId();

            var result = await _courseService.CreateCourseAsync(courseDto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateCourseRequestDto updateCourseDto)
        {
            var authorId = User.GetUserId();

            await _courseService.UpdateCourseAsync(authorId, updateCourseDto);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid courseId)
        {
            var authorId = User.GetUserId();

            await _courseService.DeleteCourseAsync(authorId, courseId);

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<CourseDto>> GetById(Guid id)
        {
            // it should also check current user has permission for the course

            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null) 
                return NotFound();

            return Ok(course);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var authorId = User.GetUserId();

            var courses = await _courseService.GetCoursesByAuthor(authorId);

            return Ok(courses);
        }
    }
}
