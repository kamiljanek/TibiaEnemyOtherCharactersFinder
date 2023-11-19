using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Middlewares;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userIpAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();

        if (context.Session.GetString("sessionId") == null)
        {
            string sessionId = Guid.NewGuid().ToString();
            context.Session.SetString("sessionId", sessionId);
        }

        context.Items["UserId"] = userId ?? userIpAddress;
        context.Items["Body"] = await ReadBodyFromRequest(context.Request);

        await _next(context);
    }

    private static async Task<string> ReadBodyFromRequest(HttpRequest request)
    {
        // ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
        request.EnableBuffering();

        using var streamreader = new StreamReader(request.Body, leaveOpen: true);
        var requestbody = await streamreader.ReadToEndAsync();

        // reset the request's body stream position for next middleware in the pipeline.
        request.Body.Position = 0;
        return requestbody;
    }
}