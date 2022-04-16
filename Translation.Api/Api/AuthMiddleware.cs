using Microsoft.Extensions.Primitives;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var hasApiKey = context.Request.Headers.TryGetValue("X-Translation-Api-Key", out StringValues apiKey);
        if (!hasApiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api key required");
            return;
        }

        var config = context.RequestServices.GetRequiredService<IConfiguration>();
        var requiredApiKey = config.GetValue<string>("ApiKey");
        if (!requiredApiKey.Equals(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Wrong api key");
            return;
        }

        await _next(context);
    }
}