using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HotelCatalog.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problemDetails = exception switch
            {
                KeyNotFoundException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Resource Not Found",
                    Detail = exception.Message,
                    Instance = context.Request.Path
                },
                ArgumentException or ArgumentNullException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Invalid Request",
                    Detail = exception.Message,
                    Instance = context.Request.Path
                },
                InvalidOperationException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Conflict",
                    Detail = exception.Message,
                    Instance = context.Request.Path
                },
                UnauthorizedAccessException => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized",
                    Detail = exception.Message,
                    Instance = context.Request.Path
                },
                _ => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Internal Server Error",
                    Detail = _environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request",
                    Instance = context.Request.Path
                }
            };

            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            if (_environment.IsDevelopment() && exception.StackTrace != null)
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            if (context.TraceIdentifier != null)
            {
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(problemDetails, options);
            await context.Response.WriteAsync(json);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}