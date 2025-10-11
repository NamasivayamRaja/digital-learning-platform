using DigitalLearningPlatform.Services.ContentService.Application.DTOs;
using DigitalLearningPlatform.Services.ContentService.Domain.Enums;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests.Controllers
{
    [Collection("SharedDbTests")]
    public class FileUploadControllerIntegrationTests : IClassFixture<ContentServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public FileUploadControllerIntegrationTests(ContentServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task InitiateAndCompleteUpload_ShouldReturnSuccess()
        {
            // First, create a course to associate the upload with
            var createCourseDto = new CreateCourseRequestDto
            {
                Title = "File Upload Test Course",
                Category = CourseCategory.Programming,
                Level = CourseLevel.AllLevels
            };
            var courseResponse = await _client.PostAsJsonAsync("/api/courses", createCourseDto);
            var course = await courseResponse.Content.ReadFromJsonAsync<CreateCourseResponseDto>();
            course.Should().NotBeNull();
            var courseId = course.Id;

            // 1. Initiate File Upload
            var initiateDto = new InitiateFileUploadRequestDto
            {
                CourseId = courseId,
                FileName = "test-video.mp4",
                ContentType = "video/mp4",
                FileSize = 1024 * 1024 * 5 // 5MB
            };
            var initiateResponse = await _client.PostAsJsonAsync("/api/file-uploads/initiate", initiateDto);
            initiateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var initiatedUpload = await initiateResponse.Content.ReadFromJsonAsync<InitiateFileUploadResponseDto>();
            initiatedUpload.Should().NotBeNull();
            initiatedUpload.UploadUrl.Should().NotBeNullOrEmpty();
            var uploadId = initiatedUpload.UploadId;

            // 2. Complete File Upload
            var completeResponse = await _client.PostAsync($"/api/file-uploads/{uploadId}/complete", null);
            completeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
