using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.Controllers;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Controller
{
    public class CourseControllerTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly CourseController _controller;
        public CourseControllerTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _controller =  new CourseController(_mockCourseService.Object);
        }

        private void SetUserContext(Guid? userId = null)
        {
            var claims = new List<Claim>();
            if (userId.HasValue)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

        }


        [Fact]
        public async Task CreateCourse_WithValidRequest_ReturnsCreatedWithInfo()
        {
            var course = new CreateCourseRequestDto
            {
                Category = CourseCategory.Business,
                Description = "Course is structured from beginners to experts.",
                Level = CourseLevel.AllLevels,
                Overview = "Overview",
                Title  = "New Title"
            };

            var authorId = Guid.NewGuid();

            SetUserContext(authorId);
            var courseId = Guid.NewGuid();
            var expectedResponse = new CreateCourseResponseDto
            (
                Guid.NewGuid(),
                course.Title,
                course.Description,
                course.Overview
            );

            _mockCourseService.Setup(x => x.CreateCourseAsync(course)).ReturnsAsync(expectedResponse);

            var result = await _controller.Create(course);

            var createdResult = result.Result as CreatedAtActionResult;

            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task CreateCourse_WhenTitleAlreadyExists_ThrowsLearningPlatformException()
        {
            SetUserContext(Guid.NewGuid());

            var courseRequestDto = new CreateCourseRequestDto
            {
                Title = "Title 1",
            };

            _mockCourseService.Setup(x => x.CreateCourseAsync(courseRequestDto))
                .ThrowsAsync(new LearningPlatformException("Course title already exist", StatusCodes.Status409Conflict));

            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Create(courseRequestDto));

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Message.Should().Be("Course title already exist");
        }

        [Fact]
        public async Task UpdateCourse_WithValidRequest_ReturnsNoContent()
        {
            var authorId = Guid.NewGuid();
            SetUserContext(authorId);

            var courseId = Guid.NewGuid();
            var updateCourse = new UpdateCourseRequestDto
            {
                Id = courseId,
                Title = "Updated Title"
            };

            _mockCourseService.Setup(x => x.UpdateCourseAsync(authorId, updateCourse))
                .Returns(async ()=> await Task.CompletedTask);

            var result =  await _controller.Update(courseId, updateCourse);
            
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task UpdateCourse_WhenTitleAlreadyExists_ThrowsLearningPlatformException()
        {
            var authorId = Guid.NewGuid();
            SetUserContext(authorId);

            var courseId = Guid.NewGuid();
            
            var updateCourseRequestDto = new UpdateCourseRequestDto
            {
                Id = courseId,
                Title = "Updated Title 1",
            };

            _mockCourseService.Setup(x => x.UpdateCourseAsync(authorId, updateCourseRequestDto))
                .ThrowsAsync(new LearningPlatformException("Course title already exist", StatusCodes.Status409Conflict));

            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Update(courseId, updateCourseRequestDto));

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Message.Should().Be("Course title already exist");
        }


        [Fact]
        public async Task UpdateCourse_WithDifferentAuthor_ThrowsLearningPlatformException()
        {
            SetUserContext(Guid.NewGuid());

            var courseId = Guid.NewGuid();
            var updateCourse = new UpdateCourseRequestDto
            {
                Id = courseId,
                Title = "Updated Title"
            };

            _mockCourseService.Setup(x => x.UpdateCourseAsync(It.IsAny<Guid>(), updateCourse))
                .ThrowsAsync(new LearningPlatformException("Course does not exist or you don't have permission to access it", StatusCodes.Status404NotFound));

            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Update(courseId, updateCourse));

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            result.Message.Should().Be("Course does not exist or you don't have permission to access it");
        }

        [Fact]
        public async Task DeleteCourse_WithValidRequest_ReturnsNoContent()
        {
            var authorId = Guid.NewGuid();
            SetUserContext(authorId);

            var courseId = Guid.NewGuid();

            _mockCourseService.Setup(x => x.DeleteCourseAsync(authorId, courseId))
                .Returns(async () => await Task.CompletedTask);

            var result = await _controller.Delete(courseId);

            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }


        [Fact]
        public async Task DeleteCourse_WithDifferentAuthor_ThrowsLearningPlatformException()
        {
            SetUserContext(Guid.NewGuid());

            var courseId = Guid.NewGuid();

            _mockCourseService.Setup(x => x.DeleteCourseAsync(It.IsAny<Guid>(), courseId))
                .ThrowsAsync(new LearningPlatformException("Course does not exist or you don't have permission to access it", StatusCodes.Status404NotFound));

            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Delete(courseId));

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            result.Message.Should().Be("Course does not exist or you don't have permission to access it");
        }

        [Fact]
        public async Task GetById_WhenCourseIsExist_ReturnsOkResult()
        {
            var courseId = Guid.NewGuid();

            var courseDto = new CourseDto
            {
                Id = courseId,
            };

            _mockCourseService.Setup(x => x.GetCourseByIdAsync(courseId)).ReturnsAsync(courseDto);

            var result = await _controller.GetById(courseId);

            var courseResult = result.Result as OkObjectResult;

            courseResult.Should().NotBeNull();
            courseResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetById_WhenCourseNotExist_ReturnsNotFoundResult()
        {

            CourseDto courseDto = null!;

            _mockCourseService.Setup(x => x.GetCourseByIdAsync(It.IsAny<Guid>())).ReturnsAsync(courseDto);

            var result = await _controller.GetById(It.IsAny<Guid>());

            var courseResult = result.Result as NotFoundResult;

            courseResult.Should().NotBeNull();
            courseResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetCourses_WhenCoursesExist_ReturnsOkResult()
        {
            var authorId = Guid.NewGuid();
            SetUserContext(authorId);

            var coursesDto = new List<CourseDto>
            {
                new CourseDto { Id = Guid.NewGuid(), Title = "Course 1" },
                new CourseDto { Id = Guid.NewGuid(), Title = "Course 2" }
            };

            _mockCourseService.Setup(x => x.GetCoursesByAuthor(authorId)).ReturnsAsync(coursesDto);

            var result = await _controller.GetCourses();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedCourses = okResult.Value as IEnumerable<CourseDto>;
            returnedCourses.Should().BeEquivalentTo(coursesDto);
        }

        [Fact]
        public async Task GetCourses_WhenNoCoursesExist_ReturnsOkResultWithEmptyList()
        {
            var authorId = Guid.NewGuid();
            SetUserContext(authorId);

            _mockCourseService.Setup(x => x.GetCoursesByAuthor(authorId)).ReturnsAsync(new List<CourseDto>());

            var result = await _controller.GetCourses();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedCourses = okResult.Value as IEnumerable<CourseDto>;
            returnedCourses.Should().BeEmpty();
        }
    }
}
