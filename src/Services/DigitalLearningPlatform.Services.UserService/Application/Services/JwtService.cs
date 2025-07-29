using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using DigitalLearningPlatform.Services.UserService.Configuration;
using DigitalLearningPlatform.Services.UserService.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DigitalLearningPlatform.Services.UserService.Application.Services
{
    public class JwtService(IOptions<JWTOptions> jwtOptions) : IJwtService
    {
        private readonly JWTOptions _jwtOptions = jwtOptions.Value;

        public string GenerateToken(User user)
        {
            if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
            {
                throw new LearningPlatformException("Jwt secret key is not configured");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
                signingCredentials:creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
