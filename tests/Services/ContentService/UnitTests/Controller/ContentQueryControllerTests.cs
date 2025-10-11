using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Controller
{
    public class ContentQueryControllerTests
    {
        private readonly Mock<ICourseContentService> _mockContentService;
        private readonly ContentQueryController _controller;

        public ContentQueryControllerTests()
        {
            _mockContentService = new Mock<ICourseContentService>();
            _controller = new ContentQueryController(_mockContentService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetCourses_WithValidParams_ReturnsOkResultWithCourses()
        {
            // Arrange
            var courseParams = new CourseParam();
            var courses = new List<CourseSummaryDto>
            {
                new CourseSummaryDto { Id = Guid.NewGuid(), Title = "Course 1" },
                new CourseSummaryDto { Id = Guid.NewGuid(), Title = "Course 2" }
            };
            var pagedList = new PagedList<CourseSummaryDto>(courses, courses.Count, 1, 10);

            _mockContentService.Setup(s => s.ListCoursesAsync(courseParams)).ReturnsAsync(pagedList);

            // Act
            var result = await _controller.GetCourses(courseParams);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedCourses = okResult.Value as PagedList<CourseSummaryDto>;
            returnedCourses.Should().BeEquivalentTo(pagedList);
        }

        [Fact]
        public async Task GetCourseDetails_WhenCourseExists_ReturnsOkResultWithDetails()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var courseDetails = new CourseDetailDto { Id = courseId, Title = "Course Details" };

            _mockContentService.Setup(s => s.GetCourseDetailAsync(courseId)).ReturnsAsync(courseDetails);

            // Act
            var result = await _controller.GetCourseDetails(courseId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedDetails = okResult.Value as CourseDetailDto;
            returnedDetails.Should().BeEquivalentTo(courseDetails);
        }

        [Fact]
        public async Task GetCourseDetails_WhenCourseDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockContentService.Setup(s => s.GetCourseDetailAsync(courseId)).ReturnsAsync((CourseDetailDto)null!);

            // Act
            var result = await _controller.GetCourseDetails(courseId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
