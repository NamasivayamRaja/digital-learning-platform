using DigitalLearningPlatform.Services.UserService.Domain;

namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}
