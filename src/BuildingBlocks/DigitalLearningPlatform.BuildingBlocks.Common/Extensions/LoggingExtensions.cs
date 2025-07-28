using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
namespace DigitalLearningPlatform.BuildingBlocks.Common.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder, string serviceName)
        {
            return hostBuilder.UseSerilog((context, loggerConfiguration) => {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithProperty("ServiceName", serviceName)
                    .WriteTo.Console()
                    .WriteTo.Seq(context.Configuration["seq:ServerUrl"]!);
            });
        }
    }
}
