namespace DigitalLearningPlatform.Services.UserService.Application.DTOs
{
    public record ProfileDto
    {
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? Avatar { get; init; }
        public string? Overview { get; set; }
        public string Role { get; init; } = string.Empty;
    }
}
