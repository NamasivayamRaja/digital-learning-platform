using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Add ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

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

app.UseHttpsRedirection();

await app.UseOcelot();

await app.RunAsync();