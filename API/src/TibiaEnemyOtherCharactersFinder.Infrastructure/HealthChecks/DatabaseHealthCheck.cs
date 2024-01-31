using Microsoft.Extensions.Diagnostics.HealthChecks;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public DatabaseHealthCheck(ITibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}