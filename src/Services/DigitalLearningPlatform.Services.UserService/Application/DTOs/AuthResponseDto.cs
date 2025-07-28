namespace DigitalLearningPlatform.Services.UserService.Application.DTOs
{
    public record AuthResponseDto
    {
        public required string Token { get; set; }

        public ProfileDto? User { get; set; } 
    }
}
