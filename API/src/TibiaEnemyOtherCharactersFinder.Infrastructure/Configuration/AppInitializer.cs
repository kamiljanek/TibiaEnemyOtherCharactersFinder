using Microsoft.Extensions.Hosting;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public class AppInitializer : IHostedService
{
    private readonly IEnumerable<IInitializer> _initializers;

    public AppInitializer(IEnumerable<IInitializer> initializers)
    {
        _initializers = initializers;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var initializer in _initializers.Where(i => i.Order.HasValue).OrderBy(i => i.Order))
        {
            await initializer.Initialize();
        }

        foreach (var initializer in _initializers.Where(i => !i.Order.HasValue))
        {
            await initializer.Initialize();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}