using DigitalLearningPlatform.Services.UserService.Application.DTOs;

namespace DigitalLearningPlatform.Services.UserService.Application.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetCurrentUserProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto profile);
    }
}
