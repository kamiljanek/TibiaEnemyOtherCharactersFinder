using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public class WorldSeeder : IWorldSeeder
{
    private const string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";

    private readonly IRepository _repository;
    private readonly ITibiaApi _tibiaApi;

    private List<string> _worldsNamesFromTibiaApi;
    private List<World> _worldsFromDb;

    public WorldSeeder(IRepository repository, ITibiaApi tibiaApi)
    {
        _repository = repository;
        _tibiaApi = tibiaApi;
    }

    public async Task SetProperties()
    {
        _worldsNamesFromTibiaApi = await _tibiaApi.FetchWorldsNamesFromTibiaApi();
        _worldsFromDb = await _repository.GetWorldsAsNoTrackingAsync();
    }

    public async Task Seed()
    {
        try
        {
            var newWorlds = _worldsNamesFromTibiaApi
                .Where(name => !_worldsFromDb.Select(world => world.Name).Contains(name))
                .Select(name =>
                {
                    return CreateWorld(name);
                })
                .ToList();

            await _repository.AddRangeAsync(newWorlds);
        }
        catch (Exception e)
        {
            Console.WriteLine(@"Cannot get worlds from ""api.tibiadata"" or from DB");
            Console.WriteLine(e);
        }
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
        try
        {
            var newWorlds = _worldsFromDb
                .Where(world => !_worldsNamesFromTibiaApi.Contains(world.Name))
            .Select(world => { world.IsAvailable = false; return world; }).ToList();

            foreach (var world in newWorlds)
            {
                await _repository.UpdateWorldAsync(world);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(@"Cannot get worlds from ""api.tibiadata""");
            Console.WriteLine(e);
        }
    }

    private World CreateWorld(string worldName)
    {
        return new World()
        {
            Name = worldName,
            Url = BuildWorldUrl(worldName),
            IsAvailable = true
        };
    }

    private string BuildWorldUrl(string worldName)
    {
        return $"{_mainUrl}&world={worldName}";
    }
}