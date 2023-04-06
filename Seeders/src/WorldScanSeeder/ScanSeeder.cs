using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public class ScanSeeder : IScanSeeder
{
    private readonly IRepository _repository;
    private readonly ITibiaDataProvider _tibiaDataProvider;

    private List<World> _availableWorlds;

    public List<World> AvailableWorlds => _availableWorlds;

    public ScanSeeder(IRepository repository, ITibiaDataProvider tibiaDataProvider)
    {
        _repository = repository;
        _tibiaDataProvider = tibiaDataProvider;
    }

    public async Task SetProperties()
    {
        _availableWorlds = await _repository.GetAvailableWorldsAsync();
    }

    public async Task Seed(World availableWorld)
    {
        var worldScan = await CreateWorldScanAsync(availableWorld);
        await _repository.AddAsync(worldScan);
    }

    private async Task<WorldScan> CreateWorldScanAsync(World world)
    {
        var charactersOnline = await _tibiaDataProvider.FetchCharactersOnlineFromTibiaApi(world.Name);

        return new WorldScan
        {
            CharactersOnline = charactersOnline.ToLower(),
            WorldId = world.WorldId,
            ScanCreateDateTime = DateTime.UtcNow,
        };
    }
}