using Moq;
using DigitalLearningPlatform.Services.UserService.Application.Services;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Domain;
using FluentAssertions;
using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;

namespace DigitalLearningPlatform.Services.UserService.Tests.Services
{
    public class ProfileServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ProfileService _service;

        public ProfileServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _service = new ProfileService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetCurrentUserProfileAsync_WithExistingUser_ReturnsProfileDto()
        {
            
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userDomain = User.Create(
                email: "Test@test.com",
                passwordHash: "passwordhash",
                roleId: roleId,
                profile: new Profile
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Avatar = "avatar.png"
                }
            );
            userDomain.Role = new Role { Name = "Learner" };

            _mockUserRepository
                .Setup(r => r.GetUserWithRoleByIdAsync(userId))
                .ReturnsAsync(userDomain);

            
            var result = await _service.GetCurrentUserProfileAsync(userId);

            _mockUserRepository.Verify(r => r.GetUserWithRoleByIdAsync(userId), Times.Once());

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new ProfileDto
            {
                FirstName = "Test",
                LastName = "Test",
                Avatar = "avatar.png",
                Email = "Test@test.com",
                Role = "Learner"
            });
        }

        [Fact]
        public async Task GetCurrentUserProfileAsync_WithNonExistentUser_ReturnsNull()
        {
            
            var userId = Guid.NewGuid();
            _mockUserRepository
                .Setup(r => r.GetUserWithRoleByIdAsync(userId))
                .ReturnsAsync((User?)null);

            
            var result = await _service.GetCurrentUserProfileAsync(userId);
            _mockUserRepository.Verify(r=> r.GetUserWithRoleByIdAsync(userId), Times.Once());
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProfileAsync_WithExistingUser_UpdatesAndSaves()
        {
            
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var existingUser = User.Create(
                email: "Test@test.com",
                passwordHash: "passwordhash",
                roleId: roleId,
                profile: new Profile
                {
                    FirstName = "OldFirst",
                    LastName = "OldLast",
                    Avatar = "oldavatar.png"
                }
            );
            var updateDto = new UpdateProfileDto
            {
                FirstName = "NewFirst",
                LastName = "NewLast",
                Avatar = "newavatar.png"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(existingUser);

            
            await _service.UpdateProfileAsync(userId, updateDto);

            _mockUserRepository.Verify(r => r.Edit(existingUser), Times.Once());
            _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once());

            Assert.Equal("NewFirst", existingUser.Profile.FirstName);
            Assert.Equal("NewLast", existingUser.Profile.LastName);
            Assert.Equal("newavatar.png", existingUser.Profile.Avatar);

        }

        [Fact]
        public async Task UpdateProfileAsync_WithNonExistentUser_ThrowsLearningPlatformException()
        {
            
            var userId = Guid.NewGuid();
            _mockUserRepository
                .Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var updateDto = new UpdateProfileDto
            {
                FirstName = "NewFirst",
                LastName = "NewLast",
                Avatar = "newavatar.png"
            };
            
            var ex = await Assert.ThrowsAsync<LearningPlatformException>(() => _service.UpdateProfileAsync(userId, updateDto));

            _mockUserRepository.Verify(r=> r.GetUserByIdAsync(userId), Times.Once());
            _mockUserRepository.Verify(r => r.Edit(null!), Times.Never());
            _mockUserRepository.Verify(r=> r.SaveChangesAsync(), Times.Never());
            ex.Should().BeOfType<LearningPlatformException>();
            ex.Message.Should().Be("User not found");

        }
    }
}