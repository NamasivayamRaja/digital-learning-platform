using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Domain;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;

namespace DigitalLearningPlatform.Services.UserService.Application.Services
{
    public class AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IJwtService jwtService) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isUserExist = await userRepository.GetUserByEmailAsync(registerDto.Email);

            if (isUserExist != null)
            {
                throw new LearningPlatformException("Email already in use!", StatusCodes.Status409Conflict);
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var role = await roleRepository.GetRoleByNameAsync("Learner") 
                ?? throw new LearningPlatformException("Default role missing");

            var user = User.Create(
                email: registerDto.Email,
                profile: new Profile
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                },
                roleId: role.Id,
                passwordHash: passwordHash);

            userRepository.Add(user);
            await userRepository.SaveChangesAsync();

            return AuthResponseMapper(user);
        }

        public async Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await userRepository.GetUserWithRoleByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new LearningPlatformException("Invalid credentials", StatusCodes.Status401Unauthorized);
            }

            return AuthResponseMapper(user);
        }

        private AuthResponseDto AuthResponseMapper(User user)
        {
            return new AuthResponseDto
            {
                Token = jwtService.GenerateToken(user),
                User = new ProfileDto
                {
                    Email = user.Email,
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                    Role = user.Role.Name
                }
            };
        }
    }
}
