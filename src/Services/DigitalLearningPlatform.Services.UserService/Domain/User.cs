using DigitalLearningPlatform.BuildingBlocks.Common.Domain;
using DigitalLearningPlatform.Services.UserService.Domain.Enums;

namespace DigitalLearningPlatform.Services.UserService.Domain
{
    public class User : AuditableEntityBase
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public UserStatus Status { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public required Profile Profile { get; set; }

        public static User Create(string email, string passwordHash, Guid roleId, Profile profile)
        {
            return new User { Email = email, PasswordHash = passwordHash, RoleId = roleId, Profile = profile };
        }
    }
}
