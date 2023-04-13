using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace WorldScanSeeder;

public class ScanSeeder : IScanSeeder
{
    private readonly IRepository _repository;
    private readonly ITibiaDataService _tibiaDataService;

    private List<World> _availableWorlds;

    public List<World> AvailableWorlds => _availableWorlds;

    public ScanSeeder(IRepository repository, ITibiaDataService tibiaDataService)
    {
        _repository = repository;
        _tibiaDataService = tibiaDataService;
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
        var charactersOnline = await _tibiaDataService.FetchCharactersOnline(world.Name);

        return new WorldScan
        {
            CharactersOnline = charactersOnline.ToLower(),
            WorldId = world.WorldId,
            ScanCreateDateTime = DateTime.UtcNow,
        };
    }
}