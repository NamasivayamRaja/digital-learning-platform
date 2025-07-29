using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningPlatform.Services.UserService.Application.DTOs
{
    public record LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }

    }
}
