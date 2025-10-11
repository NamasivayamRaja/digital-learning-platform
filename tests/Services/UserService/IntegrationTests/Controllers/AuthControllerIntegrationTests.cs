using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.MessageContracts;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Domain.Enums;
using DigitalLearningPlatform.Services.UserService.IntegrationTests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
namespace DigitalLearningPlatform.Services.UserService.IntegrationTests.Controllers
{
    [Collection("SharedDbTests")]

    public class AuthControllerIntegrationTests : IClassFixture<UserServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly UserServiceWebApplicationFactory _factory;

        public AuthControllerIntegrationTests(UserServiceWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }
        [Fact]
        public async Task Register_Then_Login_Returns_ValidResponse()
        {
            var registerDto = new RegisterDto()
            {
                Email = $"user_{Guid.NewGuid()}@example.com",
                FirstName = "Integration",
                LastName = "Tester",
                Password = "StrongP@ssword!12"
            };
            // Register
            var regResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            regResponse.EnsureSuccessStatusCode();
            var regContent = await regResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            regContent.Should().NotBeNull();
            regContent.User.Should().NotBeNull();
            regContent.User.Email.Should().Be(registerDto.Email);
            // Login
            var loginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            loginContent.Should().NotBeNull();
            loginContent.Token.Should().NotBeNullOrEmpty();
            loginContent.User.Should().NotBeNull();
            loginContent.User.Email.Should().Be(registerDto.Email);
        }

        [Fact]
        public async Task Register_AsInstructor_PublishesIntegrationEvent()
        {
            // Arrange
            var registerDto = new RegisterDto()
            {
                Email = $"instructor_{Guid.NewGuid()}@test.com",
                FirstName = "Instructor",
                LastName = "Test",
                Password = "StrongP@ssword!123",
                Role = UserRole.Instructor,
                Overview = "This is a test overview."
            };

            //Act
           var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            //Assert
            response.EnsureSuccessStatusCode();

            var eventBus = _factory.Services.GetRequiredService<IEventBus>() as MockEventBus;
            eventBus.Should().NotBeNull();
            eventBus.PublishedEvents.Should().HaveCount(1);

            var publishedEvent = eventBus.PublishedEvents.First() as InstructorRegisteredIntegrationEvent;
            publishedEvent.Should().NotBeNull();
            publishedEvent.Email.Should().Be(registerDto.Email);
            publishedEvent.FirstName.Should().Be(registerDto.FirstName);
        }


        [Fact]
        public async Task Register_WithInvalidModel_ReturnsBadRequest()
        {
            var registerDto = new RegisterDto()
            {
                Email = null!,
                FirstName = "Integration",
                LastName = "Tester",
                Password = "StrongP@ssword!12"
            };

            var registrationResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            registrationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ReturnsBadRequest()
        {
            var loginDto = new LoginDto { Email = null!, Password = "FakePassword" };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}