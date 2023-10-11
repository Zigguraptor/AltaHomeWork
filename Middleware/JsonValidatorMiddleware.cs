using System.Text.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AltaHomeWork_1.Middleware;

public class JsonValidatorMiddleware
{
    private readonly ILogger<JsonValidatorMiddleware> _logger;
    private readonly RequestDelegate _next;

    public JsonValidatorMiddleware(RequestDelegate next, ILogger<JsonValidatorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();
            if (!await TryDeserializeJson(context.Request.Body))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Bad JSON data");
                return;
            }

            context.Request.Body.Position = 0;
        }

        await _next(context);
    }

    private async Task<bool> TryDeserializeJson(Stream jsonStream)
    {
        try
        {
            await JsonSerializer.DeserializeAsync<object>(jsonStream);
        }
        catch (JsonException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("При попытки десериализовать json, возникло неожиданное исключение\n{ex}", ex);
        }

        return true;
    }
}
