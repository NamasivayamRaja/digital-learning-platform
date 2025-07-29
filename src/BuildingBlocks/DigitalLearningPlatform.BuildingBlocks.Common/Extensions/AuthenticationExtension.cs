using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;
namespace DigitalLearningPlatform.BuildingBlocks.Common.Extensions
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services
            , IConfiguration configuration
            , string environmentName) 
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var jwtKey = configuration["Jwt:SecretKey"];

                    if (string.IsNullOrWhiteSpace(jwtKey))
                    {
                        throw new LearningPlatformException("Jwt is not configured");
                    }

                    options.Authority = configuration["Jwt:Authority"];

                    options.RequireHttpsMetadata = !environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters = new TokenValidationParameters 
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero                        
                    };
                });
            return services;
        }
    }
}
