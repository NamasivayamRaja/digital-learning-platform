using DigitalLearningPlatform.Services.UserService.Application.DTOs;

namespace DigitalLearningPlatform.Services.UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto);
    }
}
