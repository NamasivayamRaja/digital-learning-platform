using DigitalLearningPlatform.ApiGateway.Aggregator;
using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Add ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddOcelot(builder.Configuration)
    .AddSingletonDefinedAggregator<CourseSectionFileInitiateAndCreateAggregator>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Issue/Bug/B3-  Documentation part not working.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.Use(async(context, next) =>
{
    string correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

    // add to current request
    context.TraceIdentifier = correlationId;
    context.Request.Headers["X-Correlation-ID"] = correlationId;

    // ensure it's passed to downstream services
    context.Items["DownstreamRequestHeaders"] = new Dictionary<string, string>()
    {
        ["X-Correlation-ID"] = correlationId,
    };

    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();

await app.UseOcelot();

await app.RunAsync();