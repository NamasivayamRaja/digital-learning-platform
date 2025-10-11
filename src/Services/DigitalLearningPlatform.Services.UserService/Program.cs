using Microsoft.EntityFrameworkCore;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Data;
using DigitalLearningPlatform.Services.UserService.Application.Services;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories.Interfaces;
using DigitalLearningPlatform.Services.UserService.Infrastructure.Repositories;
using DigitalLearningPlatform.Services.UserService.Configuration;
using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Extensions;
using Serilog.Context;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging("UserService");

builder.Services.AddDbContext<UserDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Configure Event Bus with RabbitMQ
var rabbitMQConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672/";
builder.Services.AddEventBusRabbitMQ(rabbitMQConnectionString, "UserService");

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"];

    if(!string.IsNullOrWhiteSpace(correlationId))
    {
        LogContext.PushProperty("CorrelationId", correlationId);
    }

    await next();
});

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Seeding should be placed in CI/CD pipeline. Failed the integration test runs. Keep it here for reference
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
//    db.Database.Migrate();
//    await RoleSeeder.SeedRoleAsync(db);
//}

app.Run();

public partial class Program { } // Added for Integration Test