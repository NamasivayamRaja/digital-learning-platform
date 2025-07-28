using DigitalLearningPlatform.Services.UserService.Domain;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext userDbContext) : IUserRepository
    {
        public void Add(User user)
        {
            userDbContext.Users.Add(user);
        }

        public void Edit(User user)
        {
            userDbContext.Entry(user).State = EntityState.Modified;
        }

        public async Task<User?> GetUserWithRoleByIdAsync(Guid id)
        {
            return await userDbContext.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);
        }


        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await userDbContext.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
           return await userDbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithRoleByEmailAsync(string email)
        {
            return await userDbContext.Users
                .Include(u=>u.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }


        public async Task SaveChangesAsync()
        {
            await userDbContext.SaveChangesAsync();
        }
    }
}
