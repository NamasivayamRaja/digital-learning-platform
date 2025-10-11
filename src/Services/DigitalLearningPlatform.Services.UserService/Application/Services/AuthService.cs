using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.MessageContracts;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Domain;
using DigitalLearningPlatform.Services.UserService.Domain.Enums;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;

namespace DigitalLearningPlatform.Services.UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            IJwtService jwtService,
            IEventBus eventBus,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Role.HasValue && registerDto.Role.Value == UserRole.Instructor && string.IsNullOrWhiteSpace(registerDto.Overview))
                throw new LearningPlatformException("Overview is mandatory for Instructor", 400);

            var isUserExist = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (isUserExist != null)
            {
                throw new LearningPlatformException("Email already in use!", 409);
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            registerDto.Role ??= UserRole.Learner;

            var role = await _roleRepository.GetRoleByNameAsync(registerDto.Role.Value.ToString()) 
                ?? throw new LearningPlatformException("Role setup is missing");

            var user = User.Create(
                email: registerDto.Email,
                profile: new Profile
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Overview = registerDto.Overview,
                },
                roleId: role.Id,
                passwordHash: passwordHash);

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            if (registerDto.Role == UserRole.Instructor)
            {
                await PublishInstructorRegisteredEventAsync(user);
            }

            return AuthResponseMapper(user);
        }

        public async Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserWithRoleByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new LearningPlatformException("Invalid credentials", 401);
            }

            return AuthResponseMapper(user);
        }

        private async Task PublishInstructorRegisteredEventAsync(User user)
        {
            var instructorEvent = new InstructorRegisteredIntegrationEvent(
                userId: user.Id,
                firstName: user.Profile.FirstName,
                lastName: user.Profile.LastName,
                email: user.Email,
                overview: user.Profile.Overview,
                registrationDate: user.CreatedAt);

            try
            {
                _logger.LogError("Publishing event to RabbitMQ: {EventId} {UserId}", instructorEvent.Id, instructorEvent.UserId);
                await _eventBus.PublishAsync(instructorEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing event: {EventId}", instructorEvent.Id);
            }
        }

        private AuthResponseDto AuthResponseMapper(User user)
        {
            return new AuthResponseDto
            {
                Token = _jwtService.GenerateToken(user),
                User = new ProfileDto
                {
                    Email = user.Email,
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                    Role = user.Role.Name,
                    Overview = user.Profile.Overview,
                }
            };
        }
    }
}
