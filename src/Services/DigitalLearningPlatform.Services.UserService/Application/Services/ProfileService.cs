using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
namespace DigitalLearningPlatform.Services.UserService.Application.Services
{
    public class ProfileService(IUserRepository userRepository) : IProfileService
    {
        public async Task<ProfileDto?> GetCurrentUserProfileAsync(Guid userId)
        {
            var user = await userRepository.GetUserWithRoleByIdAsync(userId);

            if (user == null) return null;

            return new ProfileDto 
            { 
                Avatar =  user.Profile.Avatar,
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                Email = user.Email,
                Role = user.Role.Name 
            };
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto updateProfileDto)
        {
            var user =  await userRepository.GetUserByIdAsync(userId) 
                ??  throw new LearningPlatformException("User not found");

            user.Profile.FirstName = updateProfileDto.FirstName;
            user.Profile.LastName = updateProfileDto.LastName;
            user.Profile.Avatar = updateProfileDto.Avatar ?? string.Empty;

            userRepository.Edit(user);

            await userRepository.SaveChangesAsync();
        }
    }
}
