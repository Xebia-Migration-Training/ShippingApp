using System.Diagnostics;

namespace ShippingRules.API.Middleware;

/// <summary>
/// Middleware to log incoming HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        // Log request
        _logger.LogInformation(
            "Incoming Request: {RequestId} {Method} {Path} from {RemoteIp}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress
        );

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log response
            _logger.LogInformation(
                "Request Completed: {RequestId} {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}

/// <summary>
/// Extension method to register the logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
