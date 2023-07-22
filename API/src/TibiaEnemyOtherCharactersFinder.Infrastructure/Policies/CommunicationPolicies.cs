using Polly;
using Polly.Extensions.Http;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;

public static class CommunicationPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetTibiaDataRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(1));
    }
}