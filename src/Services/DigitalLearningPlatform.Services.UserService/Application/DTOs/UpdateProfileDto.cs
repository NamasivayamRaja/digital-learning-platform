using System.ComponentModel.DataAnnotations;

namespace DigitalLearningPlatform.Services.UserService.Application.DTOs
{
    public record UpdateProfileDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? Avatar { get; init; }
    }
}
