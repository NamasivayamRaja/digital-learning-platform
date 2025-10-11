using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Controllers
{
    [Collection("SharedDbTests")]
    public class CourseControllerIntegrationTests : IClassFixture<ContentServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CourseControllerIntegrationTests(ContentServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CourseEndpoints_ShouldPerformCRUDOperations_Successfully()
        {
            // 1. Create Course
            var createDto = new CreateCourseRequestDto
            {
                Title = "Integration Test Course",
                Description = "A course for testing integration.",
                Overview = "Overview of the course.",
                Category = CourseCategory.Business,
                Level = CourseLevel.Beginner
            };
            var createResponse = await _client.PostAsJsonAsync("/api/courses", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCourse = await createResponse.Content.ReadFromJsonAsync<CreateCourseResponseDto>();
            createdCourse.Should().NotBeNull();
            var courseId = createdCourse.Id;

            // 2. Get Course by ID
            var getResponse = await _client.GetAsync($"/api/courses?id={courseId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetchedCourse = await getResponse.Content.ReadFromJsonAsync<CourseDto>();
            fetchedCourse.Should().NotBeNull();
            fetchedCourse.Id.Should().Be(courseId);

            // 3. Get Courses by Author
            var listResponse = await _client.GetAsync("/api/courses/list");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var courses = await listResponse.Content.ReadFromJsonAsync<List<CourseDto>>();
            courses.Should().NotBeNull().And.Contain(c => c.Id == courseId);

            // 4. Update Course
            var updateDto = new UpdateCourseRequestDto
            {
                Id = courseId,
                Title = "Updated Integration Test Course",
                Description = "An updated course description.",
                Overview = "Updated overview.",
                Category = CourseCategory.Business,
                Level = CourseLevel.Intermediate
            };
            var updateResponse = await _client.PutAsJsonAsync($"/api/courses/{courseId}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify update
            var getUpdatedResponse = await _client.GetAsync($"/api/courses?id={courseId}");
            var updatedCourse = await getUpdatedResponse.Content.ReadFromJsonAsync<CourseDto>();
            updatedCourse.Should().NotBeNull();
            updatedCourse.Title.Should().Be(updateDto.Title);

            // 5. Delete Course
            var deleteResponse = await _client.DeleteAsync($"/api/courses?courseId={courseId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify deletion
            var getDeletedResponse = await _client.GetAsync($"/api/courses?id={courseId}");
            getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
