using DigitalLearningPlatform.Services.UserService.Domain;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        // Using constructer injection traditional approach
        private readonly UserDbContext _userDbContext;

        public RoleRepository(UserDbContext userDbContext) 
        {
            _userDbContext = userDbContext;
        }
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
           return await _userDbContext.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
