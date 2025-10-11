using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Controller
{
    public class SectionControllerTests
    {
        private readonly Mock<ISectionService> _mockSectionService;
        private readonly SectionController _controller;

        public SectionControllerTests()
        {
            _mockSectionService = new Mock<ISectionService>();
            _controller = new SectionController(_mockSectionService.Object);
            SetUserContext(Guid.NewGuid());
        }

        private void SetUserContext(Guid userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task CreateDraftSection_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedResponse = new CreateSectionResponseDto(Guid.NewGuid(),Guid.NewGuid(), "Draft Title", 1, CreationStatus.Draft);
            _mockSectionService.Setup(s => s.CreateDraftSectionAsync(It.IsAny<Guid>(), courseId)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateDraftSection(courseId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Update_WithValidRequest_ReturnsNoContent()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var requestDto = new UpdateSectionRequestDto { SectionId = sectionId, CourseId = courseId, Title = "Updated Title" };
            _mockSectionService.Setup(s => s.UpdateSectionAsync(It.IsAny<Guid>(), requestDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(sectionId, requestDto);

            // Assert
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task Delete_WithValidRequest_ReturnsNoContent()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            _mockSectionService.Setup(s => s.DeleteSectionAsync(It.IsAny<Guid>(), sectionId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(sectionId);

            // Assert
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task GetSectionDetail_WhenSectionExists_ReturnsOkWithDetails()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var expectedDetails = new List<SectionFileDto>() { new() { Id = sectionId, Title = "Section Title" } };
            _mockSectionService.Setup(s => s.GetSectionDetailAsync(sectionId, It.IsAny<Guid>())).ReturnsAsync(expectedDetails);

            // Act
            var result = await _controller.GetSectionDetail(sectionId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(expectedDetails);
        }

        [Fact]
        public async Task GetSectionDetail_WhenSectionDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            _mockSectionService.Setup(s => s.GetSectionDetailAsync(sectionId, It.IsAny<Guid>())).ReturnsAsync((IEnumerable<SectionFileDto>)null!);

            // Act
            var result = await _controller.GetSectionDetail(sectionId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetSectionByCourseIdAsync_WhenSectionsExist_ReturnsOkWithSections()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedSections = new List<SectionDto> { new SectionDto(Guid.NewGuid(), "Section 1", 1, CreationStatus.Draft, DateTime.UtcNow.AddHours(-1), null) };
            _mockSectionService.Setup(s => s.GetSectionByCourseIdAndAuthorAsync(It.IsAny<Guid>(), courseId)).ReturnsAsync(expectedSections);

            // Act
            var result = await _controller.GetSectionByCourseIdAsync(courseId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(expectedSections);
        }
    }
}
