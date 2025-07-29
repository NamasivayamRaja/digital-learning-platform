using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DigitalLearningPlatform.Services.UserService.Controllers;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DigitalLearningPlatform.Services.UserService.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            var registerDto = new RegisterDto { 
                FirstName = "Test", 
                LastName = "Test", 
                Email = "Test@Test.com", 
                Password="StrongPa$$w0rd" 
            };

            var expectedResponse = new AuthResponseDto 
            { 
                Token = "Token", 
                User = new ProfileDto { 
                    Email = registerDto.Email, 
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = "Learner"
                }
            };

            _mockAuthService
                .Setup(s => s.RegisterAsync(registerDto))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Register(registerDto);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }
        
        [Fact]
        public async Task Register_WhenUserAlreadyExists_ThrowsLearningPlatformException()
        {
            var registerDto = new RegisterDto 
            { 
                FirstName = "Test", 
                LastName = "Test", 
                Email = "Test@Test.com", 
                Password = "StrongPa$$w0rd" 
            };

            _mockAuthService
                .Setup(s => s.RegisterAsync(registerDto))
                .ThrowsAsync(new LearningPlatformException("Email already in use!", StatusCodes.Status409Conflict));

            var ex = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Register(registerDto));
            ex.Message.Should().Be("Email already in use!");
            ex.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }


        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithProfileAndToken()
        {
            var loginDto = new LoginDto { Email = "Test@Test.com", Password="StrongPa$$w0rd" };
            var expectedResponse = new AuthResponseDto
            {
                Token = "Token",
                User = new ProfileDto
                {
                    Email = "Test@Test.com",
                    FirstName = "Test",
                    LastName = "Test",
                    Role = "Learner"
                }
            };

            _mockAuthService
                .Setup(s => s.AuthenticateAsync(loginDto))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Login(loginDto);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { Email = "Test@Test.com", Password = "StrongPa$$w0rd" };

            _mockAuthService
                .Setup(s => s.AuthenticateAsync(loginDto))
                .ThrowsAsync(new LearningPlatformException("Invalid credentials", StatusCodes.Status401Unauthorized));

            var ex = await Assert.ThrowsAsync<LearningPlatformException>(() => _controller.Login(loginDto));
            ex.Message.Should().Be("Invalid credentials");
            ex.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
    }
}