using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Controllers
{
    public class SectionControllerIntegrationTests : IClassFixture<ContentServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SectionControllerIntegrationTests(ContentServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SectionEndpoints_ShouldPerformCRUDOperations_Successfully()
        {
            // First, create a course to associate sections with
            var createCourseDto = new CreateCourseRequestDto 
            { 
                Title = "Section Test Course",
                Description = "A course for testing integration.",
                Overview = "Overview of the course.",
                Category = CourseCategory.Other,
                Level = CourseLevel.Advanced
            };

            var courseResponse = await _client.PostAsJsonAsync("/api/courses", createCourseDto);
            var course = await courseResponse.Content.ReadFromJsonAsync<CreateCourseResponseDto>();
            course.Should().NotBeNull();
            var courseId = course.Id;

            // 1. Create Draft Section
            var createSectionResponse = await _client.PostAsJsonAsync($"/api/courses/{courseId}/sections/drafts", courseId);
            createSectionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdSection = await createSectionResponse.Content.ReadFromJsonAsync<CreateSectionResponseDto>();
            createdSection.Should().NotBeNull();
            var sectionId = createdSection.Id;

            // 2. Get Sections by Course ID
            var getSectionsResponse = await _client.GetAsync($"/api/courses/{courseId}/sections");
            getSectionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var sections = await getSectionsResponse.Content.ReadFromJsonAsync<List<SectionDto>>();
            sections.Should().NotBeNull().And.Contain(s => s.Id == sectionId);

            // 3. Update Section
            var updateDto = new UpdateSectionRequestDto { SectionId = sectionId, Title = "Updated Section Title" };
            var updateResponse = await _client.PutAsJsonAsync($"/api/sections/{sectionId}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

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


            // 4. Get Section Detail
            var getDetailResponse = await _client.GetAsync($"/api/sections/section-files?id={sectionId}");
            getDetailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var sectionDetail = await getDetailResponse.Content.ReadFromJsonAsync<List<SectionFileDto>>();
            sectionDetail.Should().NotBeNull().And.Contain(s=> s.FileName == uploadDto.FileName);

            // 5. Delete Section
            var deleteResponse = await _client.DeleteAsync($"/api/sections/{sectionId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //// Verify deletion
            //var getDeletedDetailResponse = await _client.GetAsync($"/api/sections/section-files?id={sectionId}");
            //getDeletedDetailResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
