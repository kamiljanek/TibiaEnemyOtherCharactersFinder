using System.Globalization;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;

public abstract class AbstractRateLimiterPolicy : IRateLimiterPolicy<string>
{
    private Func<OnRejectedContext, CancellationToken, ValueTask> _onRejected;

    protected AbstractRateLimiterPolicy()
    {
        _onRejected = (context, _) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too many requests.",
                Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
                Detail = "Request limit exceeded. Please try again later.",
                Instance = context.HttpContext.Request.Path
            };

            string json = JsonSerializer.Serialize(details);
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.WriteAsync(json);

            return new ValueTask();
        };
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected => _onRejected;

    public abstract RateLimitPartition<string> GetPartition(HttpContext httpContext);
}