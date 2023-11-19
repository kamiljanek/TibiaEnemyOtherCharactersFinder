using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;

public class GlobalRateLimiterPolicy : AbstractRateLimiterPolicy, IRateLimiterPolicy<string>
{
    public GlobalRateLimiterPolicy() : base()
    {
    }

    public override RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var sessionId = httpContext.Session.Id;

        return RateLimitPartition.GetFixedWindowLimiter(sessionId,
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(5),
            });
    }
}