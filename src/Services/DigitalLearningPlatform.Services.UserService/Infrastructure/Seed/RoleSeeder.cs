using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Domain;
namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoleAsync(UserDbContext context)
        {
            var roles = new[] { "Admin", "Instructor", "Learner" };

            foreach (var role in roles)
            {
                if(!context.Roles.Any(r => r.Name == role))
                {
                    context.Roles.Add(new Role() { Name = role });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}