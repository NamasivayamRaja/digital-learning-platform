using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Controller
{
    public class FileUploadControllerTests
    {
        private readonly Mock<IFileUploadService> _mockFileUploadService;
        private readonly FileUploadController _controller;

        public FileUploadControllerTests()
        {
            _mockFileUploadService = new Mock<IFileUploadService>();
            _controller = new FileUploadController(_mockFileUploadService.Object);
        }

        [Fact]
        public async Task InitiateUpload_WithValidRequest_ReturnsOkWithUploadInfo()
        {
            // Arrange
            var requestDto = new InitiateFileUploadRequestDto { FileName = "test.mp4" };
            var responseDto = new InitiateFileUploadResponseDto { UploadUrl = "http://azure.com/upload" };

            _mockFileUploadService.Setup(s => s.InitiateUploadAsync(requestDto)).ReturnsAsync(responseDto);

            // Act
            var result = await _controller.InitiateUpload(requestDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(responseDto);
        }

        [Fact]
        public async Task CompleteUpload_WhenUploadExists_ReturnsNoContent()
        {
            // Arrange
            var uploadId = Guid.NewGuid();
            _mockFileUploadService.Setup(s => s.CompleteUploadAsync(uploadId)).ReturnsAsync(true);

            // Act
            var result = await _controller.CompleteUpload(uploadId);

            // Assert
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task CompleteUpload_WhenUploadDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var uploadId = Guid.NewGuid();
            _mockFileUploadService.Setup(s => s.CompleteUploadAsync(uploadId)).ReturnsAsync(false);

            // Act
            var result = await _controller.CompleteUpload(uploadId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
