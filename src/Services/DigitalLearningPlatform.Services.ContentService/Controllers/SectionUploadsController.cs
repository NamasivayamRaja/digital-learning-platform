using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Controllers
{
    [Authorize("Instructor")]
    [Route("api/section-uploads")]
    public class SectionUploadsController : BaseController
    {
        private readonly ISectionService _sectionService;
        private readonly IFileUploadService _fileUploadService;

        public SectionUploadsController(ISectionService sectionService, IFileUploadService fileUploadService)
        {
            _sectionService = sectionService;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Orchestrates creating a section file record and initiating a file upload.
        /// </summary>
        /// <param name="dto">The request containing file and section information.</param>
        /// <returns>A response with a pre-signed URL for the upload.</returns>
        [HttpPost("initiate")]
        public async Task<ActionResult<InitiateFileUploadResponseDto>> InitiateSectionFileUpload([FromBody] InitiateFileUploadRequestDto dto)
        {
            var authorId = User.GetUserId();

            var sectionFileId = await _sectionService.CreateSectionFileAsync(dto, authorId);

            var uploadResponse = await _fileUploadService.InitiateUploadAsync(dto);

            uploadResponse.SectionFileId = sectionFileId;

            return Ok(uploadResponse);
        }
    }
}
