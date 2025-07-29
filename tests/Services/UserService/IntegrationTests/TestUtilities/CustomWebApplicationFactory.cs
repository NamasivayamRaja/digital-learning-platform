using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var jwtSecret = Environment.GetEnvironmentVariable("Jwt_SecretKey") ?? "LONGSKEYWILLHELPUSTOAVOIDERRORJWTAUTHENTICATIONISSUESDEBUGREPEAT98765!@#$%";

            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:SecretKey", jwtSecret },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:Authority", "https://test-authority.local" }, // Optional, depending on your validation
                { "ConnectionStrings:DefaultConnection", "FakeDb" }
            });
            Console.WriteLine($"JWT Config => Secret: {jwtSecret}, Issuer: TestIssuer, Audience: TestAudience");
        });

        Console.WriteLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

        builder.ConfigureTestServices(services =>
        {
            // Remove existing registrations
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.FullName != null &&
                    (d.ServiceType.FullName.Contains("DbContextOptions") ||
                     d.ServiceType == typeof(UserDbContext)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            // Create and open SQLite in-memory connection for the entire factory lifetime
            _connection ??= new SqliteConnection("DataSource=:memory:;Cache=Shared");
            _connection.Open();

            // Register SQLite in-memory with open connection
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Ensure the database is created/seeds after the provider is swapped
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            db.Database.EnsureDeleted(); // For a clean slate
            db.Database.EnsureCreated();

            RoleSeeder.SeedRoleAsync(db).GetAwaiter().GetResult();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}