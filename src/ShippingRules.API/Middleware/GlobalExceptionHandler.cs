using System.Net;
using System.Text.Json;

namespace ShippingRules.API.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and returns a consistent, safe error response without leaking internal details.
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, new
            {
                title = "Validation Failed",
                status = (int)HttpStatusCode.BadRequest,
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteResponseAsync(context, HttpStatusCode.NotFound, new
            {
                title = "Resource Not Found",
                status = (int)HttpStatusCode.NotFound,
                detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
            {
                title = "An unexpected error occurred",
                status = (int)HttpStatusCode.InternalServerError,
                detail = "An internal server error has occurred. Please try again later."
            });
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
