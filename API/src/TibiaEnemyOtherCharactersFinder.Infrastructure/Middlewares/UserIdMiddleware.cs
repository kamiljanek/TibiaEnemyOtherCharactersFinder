using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Middlewares;

public class UserIdMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userIpAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();

        context.Items["UserId"] = userId ?? userIpAddress;

        await _next(context);
    }
}