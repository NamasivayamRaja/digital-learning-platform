using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Services;
using DigitalLearningPlatform.Services.ContentService.Domain;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _courseService = new CourseService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateCourseAsync_WithUniqueTitle_CreatesAndSavesCourse()
        {
            // Arrange
            var dto = new CreateCourseRequestDto { Title = "New Course", AuthorId = Guid.NewGuid(), Category = CourseCategory.Business, Level = CourseLevel.Beginner };
            _mockUnitOfWork.Setup(u => u.Courses.ExistsWithTitleAsync(dto.Title)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _courseService.CreateCourseAsync(dto);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(u => u.Courses.Add(It.IsAny<Course>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCourseAsync_WithDuplicateTitle_ThrowsException()
        {
            // Arrange
            var dto = new CreateCourseRequestDto { Title = "Existing Course" };
            _mockUnitOfWork.Setup(u => u.Courses.ExistsWithTitleAsync(dto.Title)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<LearningPlatformException>(() => _courseService.CreateCourseAsync(dto));
        }

        [Fact]
        public async Task DeleteCourseAsync_WithValidAuthorAndCourse_DeletesCourse()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var course = new Course("Title", "Desc", "Overview", authorId, CourseCategory.Business, CourseLevel.Beginner);
            _mockUnitOfWork.Setup(u => u.Courses.GetByIdWithAuthorizationAsync(courseId, authorId)).ReturnsAsync(course);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await _courseService.DeleteCourseAsync(authorId, courseId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Courses.Delete(course), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCourseByIdAsync_WhenCourseExists_ReturnsCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course("Title", "Desc", "Overview", Guid.NewGuid(), CourseCategory.Business, CourseLevel.Beginner);
            _mockUnitOfWork.Setup(u => u.Courses.GetByIdAsync(courseId)).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCoursesByAuthor_WhenCoursesExist_ReturnsCourses()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            var courses = new List<Course> { new Course("Title", "Desc", "Overview", authorId, CourseCategory.Business, CourseLevel.Beginner) };
            _mockUnitOfWork.Setup(u => u.Courses.GetCoursesByAuthor(authorId)).ReturnsAsync(courses);

            // Act
            var result = await _courseService.GetCoursesByAuthor(authorId);

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateCourseAsync_WithValidData_UpdatesCourse()
        {
            // Arrange
            var authorId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var dto = new UpdateCourseRequestDto { Id = courseId, Title = "Updated Title" };
            var course = new Course("Old Title", "Desc", "Overview", authorId, CourseCategory.Business, CourseLevel.Beginner);
            _mockUnitOfWork.Setup(u => u.Courses.ExistsWithTitleAsync(dto.Title, dto.Id)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.Courses.GetByIdWithAuthorizationAsync(dto.Id, authorId)).ReturnsAsync(course);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await _courseService.UpdateCourseAsync(authorId, dto);

            // Assert
            _mockUnitOfWork.Verify(u => u.Courses.Update(It.IsAny<Course>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
