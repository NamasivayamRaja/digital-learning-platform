using DigitalLearningPlatform.Services.UserService.Domain;

namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        void Edit(User user);
        Task SaveChangesAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserWithRoleByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserWithRoleByEmailAsync(string email);

    }
}
