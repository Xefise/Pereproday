using System.Text.Json;
using FluentValidation;
using Pereprodai.Shared.Application.Exceptions;

namespace Pereprodai.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(x => new ValidationError(x.PropertyName, x.ErrorMessage));
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new ErrorResponse(ex.Message, errors.ToArray())));
        }
        catch (KeyNotFoundException)
        {
            context.Response.StatusCode = 404;
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = 403;
        }
        catch (InvalidOperationException)
        {
            context.Response.StatusCode = 409;
        }
        catch (RateLimitExceededException ex)
        {
            context.Response.StatusCode = 429;
            _logger.LogInformation(ex, "RateLimitExceededException");
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new RateLimitErrorResponse(ex.Message, ex.Limit, (int)ex.Window.TotalSeconds)));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            _logger.LogError(ex, "Unhandled exception");
        }
    }
}

public record ErrorResponse(string Message, ValidationError[]? Errors = null);
public record ValidationError(string Property, string Error);

public record RateLimitErrorResponse(string Message, int Limit, int WindowSeconds);