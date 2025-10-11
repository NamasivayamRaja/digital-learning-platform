using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Application.Services;
using DigitalLearningPlatform.Services.UserService.Domain;
using DigitalLearningPlatform.Services.UserService.Domain.Enums;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.Services.UserService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _mockEventBus = new Mock<IEventBus>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _mockUserRepository.Object, 
                _mockRoleRepository.Object, 
                _mockJwtService.Object,
                _mockEventBus.Object,
                _mockLogger.Object);
        }

        // Helper Methods
        private static RegisterDto CreateRegisterDto(UserRole? role = null, string? overview = null)
        {
            return new RegisterDto
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                Password = "Test123%12",
                Role = role,
                Overview = overview
            };
        }

        [Fact]
        public async Task RegisterAsync_WithNewLearnerUser_ReturnsAuthResponseDto()
        {
            var registerDto = CreateRegisterDto();
            var token = "JWT.Token.Generated";
            var role = new Role() { Name = UserRole.Learner.ToString() };
            var user = User.Create(
                email: registerDto.Email, 
                passwordHash: "PasswordHashed",
                roleId: System.Guid.NewGuid(),
                profile: new Profile { FirstName = registerDto.FirstName, LastName= registerDto.LastName}
                );
            user.Role = role;
            AuthResponseDto expectedResponse = new() { 
                Token = token, 
                User = new ProfileDto 
                { 
                 Email = registerDto.Email,
                 FirstName = registerDto.FirstName,
                 LastName = registerDto.LastName,
                 Role = UserRole.Learner.ToString()
                }
            };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);
            _mockRoleRepository.Setup(r => r.GetRoleByNameAsync(UserRole.Learner.ToString())).ReturnsAsync(role);
            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(u => 
            {
                capturedUser = u;
                u.Role = role;
            });
            _mockJwtService.Setup(r => r.GenerateToken(It.IsAny<User>())).Returns<User>(u =>
            {
                if (u == capturedUser) return token;
                return "Wrong token";
            });

            var result = await _authService.RegisterAsync(registerDto);

            _mockUserRepository.Verify(r=> r.SaveChangesAsync(), Times.Once());
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<BuildingBlocks.MessageContracts.InstructorRegisteredIntegrationEvent>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithNewInstructorUser_PublishesEventAndReturnsAuthResponseDto()
        {
            var registerDto = CreateRegisterDto(UserRole.Instructor, "New Instructor created");
            var token = "JWT.Token.Generated";
            var role = new Role() { Name = UserRole.Instructor.ToString() };
            var user = User.Create(
                email: registerDto.Email,
                passwordHash: "PasswordHashed",
                roleId: System.Guid.NewGuid(),
                profile: new Profile { FirstName = registerDto.FirstName, LastName = registerDto.LastName }
                );
            user.Role = role;
            AuthResponseDto expectedResponse = new()
            {
                Token = token,
                User = new ProfileDto
                {
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Role = UserRole.Instructor.ToString(),
                    Overview = registerDto.Overview
                }
            };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);
            registerDto.Role.Should().NotBeNull();
            _mockRoleRepository.Setup(r => r.GetRoleByNameAsync(registerDto.Role.Value.ToString())).ReturnsAsync(role);
            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(u =>
            {
                capturedUser = u;
                u.Role = role;
            });
            _mockJwtService.Setup(r => r.GenerateToken(It.IsAny<User>())).Returns<User>(u =>
            {
                if (u == capturedUser) return token;
                return "Wrong token";
            });

            var result = await _authService.RegisterAsync(registerDto);

            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
            _mockEventBus.Verify(e => e.PublishAsync(It.Is<BuildingBlocks.MessageContracts.InstructorRegisteredIntegrationEvent>(ev => ev.UserId == capturedUser.Id)), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmailAddress_ThrowLearningPlatformException()
        {
            var registerDto = CreateRegisterDto();
            var role = new Role() { Name = UserRole.Learner.ToString() };
            var user = User.Create(
                email: registerDto.Email,
                passwordHash: "PasswordHashed",
                roleId: System.Guid.NewGuid(),
                profile: new Profile { FirstName = registerDto.FirstName, LastName = registerDto.LastName }
                );
            user.Role = role;
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync(user);
            
            var result = await Assert.ThrowsAsync<LearningPlatformException>(()=>_authService.RegisterAsync(registerDto));

            _mockRoleRepository.Verify(r => r.GetRoleByNameAsync(UserRole.Learner.ToString()), Times.Never());
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
            result.Should().BeOfType<LearningPlatformException>();
            result.Message.Should().Be("Email already in use!");
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact]
        public async Task RegisterAsync_WhenRoleSetupIsMissing_ThrowsLearningPlatformException()
        {
            var registerDto = CreateRegisterDto();
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);
            _mockRoleRepository.Setup(r => r.GetRoleByNameAsync(UserRole.Learner.ToString())).ReturnsAsync((Role?)null);
            
            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _authService.RegisterAsync(registerDto));

            result.Message.Should().Be("Role setup is missing");
            _mockUserRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Never());
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RegisterAsync_WhenOverviewFieldIsEmptyForInstructor_ThrowsLearningPlatformException()
        {
            var registerDto = CreateRegisterDto(UserRole.Instructor);
            
            var result = await Assert.ThrowsAsync<LearningPlatformException>(() => _authService.RegisterAsync(registerDto));

            result.Message.Should().Be("Overview is mandatory for Instructor");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _mockUserRepository.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never());
            _mockRoleRepository.Verify(r => r.GetRoleByNameAsync(It.IsAny<string>()), Times.Never());
            _mockUserRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Never());
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task RegisterAsync_PasswordIsProperlyHashed()
        {
            var registerDto = CreateRegisterDto();
            var role = new Role { Name = UserRole.Learner.ToString() };
            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);
            _mockRoleRepository.Setup(r => r.GetRoleByNameAsync(UserRole.Learner.ToString())).ReturnsAsync(role);
            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(u =>
            {
                capturedUser = u;
                capturedUser.Role = role;
            });
            _mockJwtService.Setup(r => r.GenerateToken(It.IsAny<User>())).Returns("token");

            await _authService.RegisterAsync(registerDto);

            capturedUser.Should().NotBeNull();
            capturedUser.PasswordHash.Should().NotBe(registerDto.Password);
            BCrypt.Net.BCrypt.Verify(registerDto.Password, capturedUser.PasswordHash).Should().BeTrue();
        }
    }
}
