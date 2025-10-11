using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Seed;
using DigitalLearningPlatform.Services.UserService.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.Extensions.Hosting; // Required for AddUserSecrets

internal static class TestConfigurationKeys
{
    public const string JwtSection = "Jwt";
    public const string SecretKey = "SecretKey";
    public const string Issuer = "Issuer";
    public const string Audience = "Audience";
    public const string Authority = "Authority";
    public const string ConnectionString = "ConnectionStrings:DefaultConnection";
}

public class UserServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DbConnection? _connection;
    public async Task InitializeAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("USERSERVICE_CONNECTIONSTRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("USERSERVICE_CONNECTIONSTRING environment variable not set.");
        }
        _connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);

        await _connection.OpenAsync();

        // Use the factory's services to get a DbContext and seed it
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        
        await dbContext.Database.MigrateAsync();
        // Clear the database before seeding
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Users]");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Roles]");

        await RoleSeeder.SeedRoleAsync(dbContext);
    }

    public new async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                 configBuilder.AddUserSecrets<UserServiceWebApplicationFactory>();
            }

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
            RemoveExistingRegistrations(services);
            services.AddSingleton<IEventBus, MockEventBus>();

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(_connection!);
            });
        });
    }

    private void RemoveExistingRegistrations(IServiceCollection services)
    {
        var descriptorsToRemove = services
            .Where(d => d.ServiceType.FullName != null &&
                (d.ServiceType.FullName.Contains("DbContextOptions") ||
                 d.ServiceType == typeof(UserDbContext) ||
                 d.ServiceType.FullName.Contains("RabbitMQ") ||
                 d.ServiceType.FullName.Contains("IEventBus")))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
            services.Remove(descriptor);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}