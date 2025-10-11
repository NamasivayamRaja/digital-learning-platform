using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
namespace DigitalLearningPlatform.Services.UserService.IntegrationTests.Controllers
{
    [Collection("SharedDbTests")]
    public class ProfileControllerIntegrationTests : IClassFixture<UserServiceWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public ProfileControllerIntegrationTests(UserServiceWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        // Helper to register/login and get JWT
        private async Task<string> RegisterAndLoginAndGetJwtAsync(RegisterDto registerDto)
        {
            // Register
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            regResp.EnsureSuccessStatusCode();
            // Login
            var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new LoginDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            });
            loginResp.EnsureSuccessStatusCode();
            var loginContent = await loginResp.Content.ReadFromJsonAsync<AuthResponseDto>();
            loginContent.Should().NotBeNull();
            loginContent.Token.Should().NotBeNullOrEmpty();
            return loginContent.Token;
        }
        [Fact]
        public async Task GetProfile_AuthorizedUser_ReturnsProfile()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = $"user_{Guid.NewGuid()}@example.com",
                FirstName = "IntProfile",
                LastName = "Tester",
                Password = "StrongP@ssword!78"
            };
            var jwt = await RegisterAndLoginAndGetJwtAsync(registerDto);
            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var response = await _client.GetAsync("/api/profile");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();
            profile.Should().NotBeNull();
            profile.Email.Should().Be(registerDto.Email);
            profile.FirstName.Should().Be(registerDto.FirstName);
            profile.LastName.Should().Be(registerDto.LastName);
        }
        [Fact]
        public async Task UpdateProfile_AuthorizedUser_UpdatesAndReturnsNoContent()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = $"update_user_{Guid.NewGuid()}@example.com",
                FirstName = "ToBeUpdated",
                LastName = "User",
                Password = "StrongP@ssword!88"
            };
            var jwt = await RegisterAndLoginAndGetJwtAsync(registerDto);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var updateProfile = new UpdateProfileDto
            {
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                Avatar = "updatedavatar.png"
            };
            // Act - Update Profile
            var putResponse = await _client.PutAsJsonAsync("/api/profile", updateProfile);
            // Assert
            putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            // Act - verify updated profile
            var getResponse = await _client.GetAsync("/api/profile");
            var profile = await getResponse.Content.ReadFromJsonAsync<ProfileDto>();
            profile.Should().NotBeNull();
            profile.FirstName.Should().Be(updateProfile.FirstName);
            profile.LastName.Should().Be(updateProfile.LastName);
            profile.Avatar.Should().Be(updateProfile.Avatar);
        }
        [Fact]
        public async Task ProfileEndpoints_Returns401_WhenMissingOrInvalidToken()
        {
            // No auth header
            var getResp = await _client.GetAsync("/api/profile");
            getResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            // Set invalid JWT
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.real.jwt");
            var updateProfile = new UpdateProfileDto
            {
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                Avatar = "updatedavatar.png"
            };
            // Act - Update Profile
            var putResponse = await _client.PutAsJsonAsync("/api/profile", updateProfile);
            putResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
