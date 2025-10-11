using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Controllers
{
    [Authorize("Instructor")]
    [Route("api/sections")]
    public class SectionController : BaseController
    {
        private readonly ISectionService _sectionService;
        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpPost("/api/courses/{courseId:guid}/sections/drafts")]
        public async Task<ActionResult<CreateSectionResponseDto>> CreateDraftSection([FromRoute] Guid courseId)
        {
            var authorId = User.GetUserId();

            var result = await _sectionService.CreateDraftSectionAsync(authorId, courseId);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSectionRequestDto saveSectionDto)
        {
            var authorId = User.GetUserId();

            await _sectionService.UpdateSectionAsync(authorId, saveSectionDto);

            return NoContent(); 
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {

            var authorId = User.GetUserId();

            await _sectionService.DeleteSectionAsync(authorId,id);

            return NoContent();
        }

        [HttpGet("section-files")]
        public async Task<ActionResult<IEnumerable<SectionFileDto>>> GetSectionDetail([FromQuery] Guid id)
        {
            var authorId = User.GetUserId();

            var result = await _sectionService.GetSectionDetailAsync(id, authorId);

            if(result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("/api/courses/{courseId:guid}/sections")]
        public async Task<ActionResult<IEnumerable<SectionDto>>> GetSectionByCourseIdAsync([FromRoute] Guid courseId, CreationStatus? status =  null)
        {
            var authorId = User.GetUserId();

            var result =  await _sectionService.GetSectionByCourseIdAndAuthorAsync(authorId,  courseId);

            return Ok(result);
        }

    }
}
