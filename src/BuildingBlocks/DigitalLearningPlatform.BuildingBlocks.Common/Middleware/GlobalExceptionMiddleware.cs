using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using DigitalLearningPlatform.BuildingBlocks.Common.Exceptions;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                var requestId = context.TraceIdentifier;
                _logger.LogWarning(ex, $"Domain Exception {requestId}");
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);
            }
            catch (LearningPlatformException ex)
            {
                var requestId = context.TraceIdentifier;
                _logger.LogError(ex, $"Learning Platform Exception {requestId}");
                await HandleExceptionAsync(context, ex, ex.StatusCode);
            }
            catch (Exception ex) {
                var requestId = context.TraceIdentifier;
                _logger.LogCritical(ex, $"Unhandled Exception {requestId}");

                await HandleExceptionAsync(context, ex
                    ,StatusCodes.Status500InternalServerError
                    ,"An unexpected error occurred. Reference" + requestId);

            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode, string? message = null) 
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = message ?? exception.Message
            }));
        }
    }
}
