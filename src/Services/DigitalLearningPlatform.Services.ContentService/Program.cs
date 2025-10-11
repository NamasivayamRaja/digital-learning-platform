using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Abstractions;
using DigitalLearningPlatform.BuildingBlocks.EventBus.Extensions;
using DigitalLearningPlatform.BuildingBlocks.MessageContracts;
using DigitalLearningPlatform.Services.ContentService.Application.EventHandlers;
using DigitalLearningPlatform.Services.ContentService.Extensions;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging("ContentService");

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.EnvironmentName);

var rabbitMQConnectionString = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672/";
builder.Services.AddEventBusRabbitMQ(rabbitMQConnectionString, "ContentService");

var app = builder.Build();

app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"];
    if (!string.IsNullOrWhiteSpace(correlationId))
    {
        LogContext.PushProperty("CorrelationId", correlationId);
    }
    await next();
});

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

ConfigureEventBusSubscriptions(app);

app.Run();

static void ConfigureEventBusSubscriptions(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
    var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Configuring Event Bus subscriptions for ContentService...");
        eventBus.SubscribeAsync<InstructorRegisteredIntegrationEvent, InstructorRegisteredIntegrationEventHandler>();
        logger.LogInformation("Successfully configured Event Bus subscriptions.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to configure Event Bus subscriptions.");
        throw;
    }
}

public partial class Program { }
