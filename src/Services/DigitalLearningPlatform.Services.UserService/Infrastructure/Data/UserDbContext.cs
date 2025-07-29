using DigitalLearningPlatform.Services.UserService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().OwnsOne(u => u.Profile);

            //key and constraint config
            modelBuilder.Entity<User>().HasIndex(u => u.Email);
            modelBuilder.Entity<Role>().HasIndex(r => r.Name);

            base.OnModelCreating(modelBuilder);
        }
    }
}
