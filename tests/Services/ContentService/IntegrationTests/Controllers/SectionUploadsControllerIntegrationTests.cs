using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Controllers
{
    public class SectionUploadsControllerIntegrationTests : IClassFixture<ContentServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SectionUploadsControllerIntegrationTests(ContentServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task InitiateSectionFileUpload_ShouldReturnSuccess()
        {
            // First, create a course and a section
            var courseDto = new CreateCourseRequestDto
            {
                Title = "Course for Section Upload",
                Description = "A course for testing integration.",
                Overview = "Overview of the course.",
                Category = CourseCategory.Design,
                Level = CourseLevel.Beginner
            };

            var courseResponse = await _client.PostAsJsonAsync("/api/courses", courseDto);
            var course = await courseResponse.Content.ReadFromJsonAsync<CreateCourseResponseDto>();
            course.Should().NotBeNull();
            var courseId = course.Id;

            var sectionResponse = await _client.PostAsJsonAsync($"/api/courses/{courseId}/sections/drafts", courseId);
            var section = await sectionResponse.Content.ReadFromJsonAsync<CreateSectionResponseDto>();
            section.Should().NotBeNull();
            var sectionId = section.Id;

            // Now, initiate the upload
            var uploadDto = new InitiateFileUploadRequestDto
            {
                SectionId = sectionId,
                FileName = "lecture.pdf",
                ContentType = "application/pdf",
                FileSize = 1024 * 2, // 2KB
                CourseId = courseId
            };
            var response = await _client.PostAsJsonAsync("/api/section-uploads/initiate", uploadDto);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<InitiateFileUploadResponseDto>();
            result.Should().NotBeNull();
            result.UploadUrl.Should().NotBeNullOrEmpty();
        }
    }
}
