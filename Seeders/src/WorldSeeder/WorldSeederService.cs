using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace WorldSeeder;

public class WorldSeederService : IWorldSeederService
{
    private const string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";

    private readonly IRepository _repository;
    private readonly ITibiaDataService _tibiaDataService;

    private List<string> _worldsNamesFromTibiaDataProvider;
    private List<World> _worldsFromDb;

    public WorldSeederService(IRepository repository, ITibiaDataService tibiaDataService)
    {
        _repository = repository;
        _tibiaDataService = tibiaDataService;
    }

    public async Task SetProperties()
    {
        _worldsNamesFromTibiaDataProvider = await _tibiaDataService.FetchWorldsNames();
        _worldsFromDb = await _repository.GetWorldsAsNoTrackingAsync();
    }

    public async Task Seed()
    {
            var newWorlds = _worldsNamesFromTibiaDataProvider
                .Where(name => !_worldsFromDb.Select(world => world.Name).Contains(name))
                .Select(name =>
                {
                    return CreateWorld(name);
                })
                .ToList();

            await _repository.AddRangeAsync(newWorlds);
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
            var newWorlds = _worldsFromDb
                .Where(world => !_worldsNamesFromTibiaDataProvider.Contains(world.Name))
            .Select(world => { world.IsAvailable = false; return world; }).ToList();

            foreach (var world in newWorlds)
            {
                await _repository.UpdateWorldAsync(world);
            }
    }

    private World CreateWorld(string worldName)
    {
        return new World()
        {
            Name = worldName,
            Url = BuildWorldUrl(worldName),
            IsAvailable = false
        };
    }

    private string BuildWorldUrl(string worldName)
    {
        return $"{_mainUrl}&world={worldName}";
    }
}