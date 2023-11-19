using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;

public class PromptRateLimiterPolicy : AbstractRateLimiterPolicy, IRateLimiterPolicy<string>
{
    public PromptRateLimiterPolicy() : base()
    {
    }

    public override RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var sessionId = httpContext.Session.Id;

        return RateLimitPartition.GetFixedWindowLimiter(sessionId,
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 30,
                Window = TimeSpan.FromSeconds(10),
            });
    }
}