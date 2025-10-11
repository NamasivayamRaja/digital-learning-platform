using DigitalLearningPlatform.BuildingBlocks.Common.Helper;
using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Application.Params;
using DigitalLearningPlatform.Services.ContentService.Application.Services;
using DigitalLearningPlatform.Services.ContentService.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.UnitTests.Services
{
    public class ContentServiceTests
    {
        private readonly Mock<IContentQueryRepository> _mockContentQueryRepository;
        private readonly DigitalLearningPlatform.Services.ContentService.Application.Services.CourseContentService _contentService;

        public ContentServiceTests()
        {
            _mockContentQueryRepository = new Mock<IContentQueryRepository>();
            _contentService = new DigitalLearningPlatform.Services.ContentService.Application.Services.CourseContentService(_mockContentQueryRepository.Object);
        }

        [Fact]
        public async Task ListCoursesAsync_WithValidParams_ReturnsPagedListOfCourses()
        {
            // Arrange
            var courseParam = new CourseParam();
            var courses = new List<CourseSummaryDto> { new CourseSummaryDto { Id = Guid.NewGuid(), Title = "Course 1" } };
            var pagedList = new PagedList<CourseSummaryDto>(courses, courses.Count, 1, 10);
            _mockContentQueryRepository.Setup(r => r.ListCoursesAsync(courseParam)).ReturnsAsync(pagedList);

            // Act
            var result = await _contentService.ListCoursesAsync(courseParam);

            // Assert
            result.Should().BeEquivalentTo(pagedList);
        }

        [Fact]
        public async Task GetCourseDetailAsync_WhenCourseExists_ReturnsCourseDetails()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var courseDetails = new CourseDetailDto { Id = courseId, Title = "Course Details" };
            _mockContentQueryRepository.Setup(r => r.GetCourseWithSectionsAndFilesAsync(courseId)).ReturnsAsync(courseDetails);

            // Act
            var result = await _contentService.GetCourseDetailAsync(courseId);

            // Assert
            result.Should().BeEquivalentTo(courseDetails);
        }

        [Fact]
        public async Task GetCourseDetailAsync_WhenCourseDoesNotExist_ReturnsNull()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockContentQueryRepository.Setup(r => r.GetCourseWithSectionsAndFilesAsync(courseId)).ReturnsAsync((CourseDetailDto)null!);

            // Act
            var result = await _contentService.GetCourseDetailAsync(courseId);

            // Assert
            result.Should().BeNull();
        }
    }
}
