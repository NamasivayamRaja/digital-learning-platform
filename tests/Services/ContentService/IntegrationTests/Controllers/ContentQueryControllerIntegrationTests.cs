using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Controllers
{
    [Collection("SharedDbTests")]
    public class ContentQueryControllerIntegrationTests : IClassFixture<ContentServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContentQueryControllerIntegrationTests(ContentServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCourses_And_GetCourseDetails_ShouldReturnSuccess()
        {
            // First, create a course to ensure there is data to query
            var createDto = new CreateCourseRequestDto
            {
                Title = "Query Test Course",
                Description = "A course for testing queries.",
                Overview = "Overview for query test.",
                Category = CourseCategory.Design,
                Level = CourseLevel.AllLevels
            };
            var createResponse = await _client.PostAsJsonAsync("/api/courses", createDto);
            
            createResponse.EnsureSuccessStatusCode(); // Ensure course creation is successful
            var createdCourse = await createResponse.Content.ReadFromJsonAsync<CreateCourseResponseDto>();
            createdCourse.Should().NotBeNull();
            var courseId = createdCourse.Id;

            // 1. Get Courses (List)
            var listResponse = await _client.GetAsync("/api/ContentQuery");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var courses = await listResponse.Content.ReadFromJsonAsync<List<CourseSummaryDto>>();
            courses.Should().NotBeNull();
            courses.Should().Contain(c => c.Id == courseId);

            // 2. Get Course Details
            var detailResponse = await _client.GetAsync($"/api/ContentQuery/{courseId}/details");
            detailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var courseDetails = await detailResponse.Content.ReadFromJsonAsync<CourseDetailDto>();
            courseDetails.Should().NotBeNull();
            courseDetails.Id.Should().Be(courseId);
        }

        // Helper class to deserialize PagedList<T>
        private class PagedListTest<T>
        {
            public List<T>? Items { get; set; }
        }
    }
}
