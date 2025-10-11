using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Controller
{
    public class SectionUploadsControllerTests
    {
        private readonly Mock<ISectionService> _mockSectionService;
        private readonly Mock<IFileUploadService> _mockFileUploadService;
        private readonly SectionUploadsController _controller;

        public SectionUploadsControllerTests()
        {
            _mockSectionService = new Mock<ISectionService>();
            _mockFileUploadService = new Mock<IFileUploadService>();
            _controller = new SectionUploadsController(_mockSectionService.Object, _mockFileUploadService.Object);
            SetUserContext(Guid.NewGuid());
        }

        private void SetUserContext(Guid userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task InitiateSectionFileUpload_WithValidRequest_ReturnsOkWithUploadInfo()
        {
            // Arrange
            var requestDto = new InitiateFileUploadRequestDto { FileName = "test.mp4", SectionId = Guid.NewGuid() };
            var sectionFileId = Guid.NewGuid();
            var uploadResponse = new InitiateFileUploadResponseDto { SectionFileId = sectionFileId, UploadUrl = "http://azure.com/upload" };

            _mockSectionService.Setup(s => s.CreateSectionFileAsync(requestDto, It.IsAny<Guid>())).ReturnsAsync(sectionFileId);
            _mockFileUploadService.Setup(s => s.InitiateUploadAsync(requestDto)).ReturnsAsync(uploadResponse);

            // Act
            var result = await _controller.InitiateSectionFileUpload(requestDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            
            var responseValue = okResult.Value;
            responseValue.Should().NotBeNull();
            responseValue.Should().BeEquivalentTo(uploadResponse);
            //var sectionFileIdProperty = responseValue.GetType().GetProperty("sectionFileId");
            //sectionFileIdProperty.Should().NotBeNull();
            //sectionFileIdProperty.GetValue(responseValue).Should().Be(sectionFileId);

            //var uploadResponseProperty = responseValue.GetType().GetProperty("uploadResponse");
            //uploadResponseProperty.Should().NotBeNull();
            //uploadResponseProperty.GetValue(responseValue).Should().BeEquivalentTo(uploadResponse);
        }
    }
}
