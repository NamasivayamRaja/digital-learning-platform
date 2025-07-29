using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
namespace DigitalLearningPlatform.Services.UserService.IntegrationTests.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task Register_Then_Login_Returns_ValidResponse()
        {
            var registerDto = new RegisterDto()
            {
                Email = "integrationtest@example.com",
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