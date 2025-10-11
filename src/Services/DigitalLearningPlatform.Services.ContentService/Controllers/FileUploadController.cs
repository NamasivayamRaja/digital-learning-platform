using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.ContentService.Controllers
{
    [Authorize("Instructor")]
    [Route("api/file-uploads")]
    public class FileUploadController : BaseController
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Initiates an upload and returns a presigned/SAS URL
        /// It's not be used on client code keep it here for reference
        /// </summary>
        [HttpPost("initiate")]
        public async Task<ActionResult<InitiateFileUploadResponseDto>> InitiateUpload([FromBody] InitiateFileUploadRequestDto dto)
        {
            var response = await _fileUploadService.InitiateUploadAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint for client to notify backend upload is finished (if needed).
        /// </summary>
        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteUpload([FromRoute]Guid id)
        {
            var result =  await _fileUploadService.CompleteUploadAsync(id);

            if(!result)
                return NotFound();

            return NoContent();
        }

        /// Enhancement
        /// <summary>
        /// Get the status of a file upload by upload ID.
        /// </summary>
    }
}