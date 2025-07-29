using DigitalLearningPlatform.BuildingBlocks.Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Extensions
{
    public static class GlobalExceptionMiddlewareExtension
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
