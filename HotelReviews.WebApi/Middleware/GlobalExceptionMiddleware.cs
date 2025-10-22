using FluentValidation;
using DomainExceptions = HotelReviews.Domain.Exceptions;  
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;

namespace HotelReviews.WebApi.Middleware;
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Необроблений виняток: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            DomainExceptions.NotFoundException notFoundEx => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Ресурс не знайдено",
                Detail = notFoundEx.Message,
                Instance = context.Request.Path
            },

            DomainExceptions.ConflictException conflictEx => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Конфлікт бізнес-правил",
                Detail = conflictEx.Message,
                Instance = context.Request.Path
            },

            FluentValidation.ValidationException validationEx => new ValidationProblemDetails(
                validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Помилка валідації",
                Instance = context.Request.Path
            },

            DomainExceptions.ValidationException domainValidationEx =>
                new ValidationProblemDetails(domainValidationEx.Errors)
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Помилка валідації",
                    Detail = domainValidationEx.Message,
                    Instance = context.Request.Path
                },

            MongoConnectionException mongoConnectionEx => new ProblemDetails
            {
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Title = "Помилка підключення до бази даних",
                Detail = _environment.IsDevelopment()
                    ? mongoConnectionEx.Message
                    : "Тимчасові проблеми з базою даних. Спробуйте пізніше.",
                Instance = context.Request.Path
            },

            MongoWriteException mongoWriteEx when mongoWriteEx.WriteError.Category == ServerErrorCategory.DuplicateKey
                => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Дублікат запису",
                    Detail = "Запис з такими даними вже існує",
                    Instance = context.Request.Path
                },

            MongoException mongoEx => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Помилка бази даних",
                Detail = _environment.IsDevelopment()
                    ? mongoEx.Message
                    : "Виникла помилка при роботі з базою даних",
                Instance = context.Request.Path
            },

            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Внутрішня помилка сервера",
                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "Виникла непередбачена помилка",
                Instance = context.Request.Path
            }
        };

        if (_environment.IsDevelopment() && exception.StackTrace != null)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        }

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }
}
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}