using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace DigitalLearningPlatform.Services.UserService.Test.Controllers
{
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _controller = new ProfileController(_mockProfileService.Object);
        }

        private void SetUserContext(Guid? userId = null)
        {
            var claims = new List<Claim>();
            if (userId.HasValue)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public async Task GetProfile_WithValidUser_ReturnsOkWithProfile()
        {

            var userId = Guid.NewGuid();
            SetUserContext(userId);
            var expectedProfile = new ProfileDto
            {
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = "Learner"
            };
            _mockProfileService
                .Setup(x => x.GetCurrentUserProfileAsync(userId))
                .ReturnsAsync(expectedProfile);


            var result = await _controller.GetProfile();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedProfile);
        }

        [Fact]
        public async Task GetProfile_WhenProfileNotFound_ReturnsNotFound()
        {

            var userId = Guid.NewGuid();
            SetUserContext(userId);

            _mockProfileService
                .Setup(x => x.GetCurrentUserProfileAsync(userId))
                .ReturnsAsync((ProfileDto?)null);

            var result = await _controller.GetProfile();

            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProfile_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
        {

            SetUserContext(null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _controller.GetProfile();
            });
        }

        [Fact]
        public async Task UpdateProfile_WithValidData_ReturnsNoContent()
        {

            var userId = Guid.NewGuid();
            SetUserContext(userId);
            var updateDto = new UpdateProfileDto
            {
                FirstName = "Updated",
                LastName = "User",
                Avatar = "avatar.png"
            };

            _mockProfileService
                .Setup(x => x.UpdateProfileAsync(userId, updateDto))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateProfile(updateDto);

            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(204);
            _mockProfileService.Verify(x => x.UpdateProfileAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_WhenUserIdMissing_ThrowsUnauthorizedAccessException()
        {
            SetUserContext(null);
            var updateDto = new UpdateProfileDto { FirstName = "", LastName = "" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _controller.UpdateProfile(updateDto);
            });
        }
    }
}