using System.ComponentModel.DataAnnotations;

namespace DigitalLearningPlatform.Services.UserService.Application.DTOs
{
    public record RegisterDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain a letter, a number, and a special character.")]

        [Required]
        public required string Password { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
    }
}
