using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public class WorldSeeder : IWorldSeeder
{
    private const string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";

    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly IRepository _worldRepository;
    private readonly ITibiaApi _tibiaApi;

    private readonly List<string> _worldsNamesFromTibiaApi = new();
    private readonly List<World> _worldsFromDb = new();

    public WorldSeeder(ITibiaCharacterFinderDbContext dbContext, IRepository worldRepository, ITibiaApi tibiaApi)
    {
        _dbContext = dbContext;
        _worldRepository = worldRepository;
        _tibiaApi = tibiaApi;
    }

    public async Task SetProperties()
    {
        _worldsNamesFromTibiaApi.AddRange(await _tibiaApi.FetchWorldsNamesFromTibiaApi());
        _worldsFromDb.AddRange(await _worldRepository.GetWorldsAsNoTrackingAsync());
    }

    public async Task Seed()
    {
        if (_dbContext.Database.CanConnect())
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

                _dbContext.Worlds.AddRange(newWorlds);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Cannot get worlds from ""api.tibiadata"" or from DB");
                Console.WriteLine(e);
            }
        }
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
        if (_dbContext.Database.CanConnect())
        {
            try
            {
                _worldsFromDb
                    .Where(world => !_worldsNamesFromTibiaApi.Contains(world.Name))
                    .Select(world => world.IsAvailable = false);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Cannot get worlds from ""api.tibiadata""");
                Console.WriteLine(e);
            }
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