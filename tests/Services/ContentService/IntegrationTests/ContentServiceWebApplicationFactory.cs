using DigitalLearningPlatform.Services.ContentService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using DigitalLearningPlatform.Services.ContentService.Application.Interfaces;
using DigitalLearningPlatform.Services.ContentService.IntegrationTests.Mocks;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.TestHost;

internal static class TestConfigurationKeys
{
    public const string JwtSection = "Jwt";
    public const string SecretKey = "SecretKey";
    public const string Issuer = "Issuer";
    public const string Audience = "Audience";
    public const string Authority = "Authority";
    public const string ConnectionString = "ConnectionStrings:DefaultConnection";
}


namespace DigitalLearningPlatform.Services.ContentService.IntegrationTests
{
    public class ContentServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
            await dbContext.Database.MigrateAsync();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Courses\"");
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"AuthorLookUps\"");

            await dbContext.SaveChangesAsync();

            await dbContext.Database.ExecuteSqlRawAsync(@"
                INSERT INTO ""AuthorLookUps"" (""AuthorId"", ""AuthorName"")
                VALUES ('a845a385-c49b-4e0e-92b4-375936499a99', 'Test Instructor')
                ON CONFLICT (""AuthorId"") DO NOTHING;
            ");

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        public new Task DisposeAsync() => Task.CompletedTask;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { $"{TestConfigurationKeys.JwtSection}:{TestConfigurationKeys.Issuer}", "TestIssuer" },
                    { $"{TestConfigurationKeys.JwtSection}:{TestConfigurationKeys.Audience}", "TestAudience" },
                    { $"{TestConfigurationKeys.JwtSection}:{TestConfigurationKeys.Authority}", "https://test-authority.local" },
                    { TestConfigurationKeys.ConnectionString, "FakeDb" }
                });
            });

            builder.ConfigureTestServices(services =>
            {
                var descriptorsToRemove = services
                        .Where(d => d.ServiceType.FullName != null &&
                        (
                             d.ServiceType.FullName.Contains("DbContextOptions") ||
                             d.ServiceType == typeof(CourseDbContext) ||
                             d.ServiceType.FullName.Contains("RabbitMQ") ||
                             d.ServiceType.FullName.Contains("IEventBus")
                        )).ToList();

                foreach (var descriptor in descriptorsToRemove)
                    services.Remove(descriptor);

                var connectionString = Environment.GetEnvironmentVariable("CONTENTSERVICE_CONNECTIONSTRING");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("CONTENTSERVICE_CONNECTIONSTRING environment variable not set.");
                }

                services.AddDbContext<CourseDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });

                services.AddSingleton<IEventBus, MockEventBus>();


                services.AddAuthentication(defaultScheme: "Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Instructor", policy =>
                        policy.RequireAuthenticatedUser()
                              .RequireClaim("role", "Instructor"));
                });

                services.AddScoped<IStorageService, MockStorageService>();
            });

            builder.UseEnvironment("Development");
        }
    }
}
