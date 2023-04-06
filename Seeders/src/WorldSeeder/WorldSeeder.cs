using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public class WorldSeeder : IWorldSeeder
{
    private const string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";

    private readonly IRepository _repository;
    private readonly ITibiaDataProvider _tibiaDataProvider;

    private List<string> _worldsNamesFromTibiaDataProvider;
    private List<World> _worldsFromDb;

    public WorldSeeder(IRepository repository, ITibiaDataProvider tibiaDataProvider)
    {
        _repository = repository;
        _tibiaDataProvider = tibiaDataProvider;
    }

    public async Task SetProperties()
    {
        _worldsNamesFromTibiaDataProvider = await _tibiaDataProvider.FetchWorldsNamesFromTibiaApi();
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