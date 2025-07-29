using DigitalLearningPlatform.Services.UserService.Domain;

namespace DigitalLearningPlatform.Services.UserService.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
