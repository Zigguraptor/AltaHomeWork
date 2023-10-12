#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Diagnostics;

namespace AltaHomeWork_1.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    private readonly Stopwatch _stopwatch = new();

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _stopwatch.Restart();
        await _next(context);
        _stopwatch.Stop();
        _logger.LogInformation("Запрос обработан за {ms}ms", _stopwatch.ElapsedMilliseconds.ToString());
    }
}
