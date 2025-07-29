using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
using DigitalLearningPlatform.Services.UserService.Application.Services;
using DigitalLearningPlatform.Services.UserService.Configuration;
using DigitalLearningPlatform.Services.UserService.Domain;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DigitalLearningPlatform.Services.UserService.Tests.Services
{
    public class JwtServiceTests
    {
        private static JWTOptions CreateJwtOptions(string? secretKey)
        {
            var jwtOptions = new JWTOptions
            {
                SecretKey = secretKey!,
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiresMinutes = 30
            };

            return jwtOptions;
        }
        public static User CreateUser(Guid roleId)
        {
            return User.Create
                (
                    email: "Test@test.com",
                    passwordHash: "passwordhash",
                    roleId: roleId,
                    profile: new Profile
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Avatar = "avatar.png"
                    }
                );
        }

        [Fact]
        public void GenerateToken_WithValidUser_ReturnsValidJwt()
        {
            var roleId = Guid.NewGuid();

            User userDomain = CreateUser(roleId);

            userDomain.Role = new Role { Name = "Learner" };

            var jwtOptions = CreateJwtOptions("ValidSecretKeyWithMoreThan16Characters");

            var jwtService = new JwtService(Options.Create(jwtOptions));


            var token = jwtService.GenerateToken(userDomain);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            token.Should().NotBeNullOrEmpty();
            jsonToken.Should().NotBeNull();
            jsonToken.Issuer.Should().Be(jwtOptions.Issuer);
            jsonToken.Audiences.Should().HaveCount(1);
            jsonToken.Audiences.Should().Contain(c=>c == jwtOptions.Audience);
            //jsonToken.Claims.Should().HaveCount(3); sub,expiry, audience, iss
            jsonToken.Claims.Should().Contain(cl => cl.Type == JwtRegisteredClaimNames.Sub && cl.Value == userDomain.Id.ToString());
            jsonToken.Claims.Should().Contain(cl => cl.Type == ClaimTypes.Email && cl.Value == userDomain.Email);
            jsonToken.Claims.Should().Contain(cl => cl.Type == ClaimTypes.Role && cl.Value == userDomain.Role.Name);
            var expectedExpiry = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiresMinutes);
            jsonToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));

        }

        [Fact]
        public void GenerateToken_WithOutSecretKey_ThrowsLearningPlatformException()
        {
            var jwtOptions = CreateJwtOptions(null);
            var jwtService = new JwtService(Options.Create(jwtOptions));
            var roleId = Guid.NewGuid();
            var user = CreateUser(roleId);
            var ex = Assert.Throws<LearningPlatformException>(()=>jwtService.GenerateToken(user));
            ex.Should().BeOfType<LearningPlatformException>();
            ex.Message.Should().Be("Jwt secret key is not configured");
        }

    }
}
