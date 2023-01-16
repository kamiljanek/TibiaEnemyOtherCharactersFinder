using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public class WorldScanSeeder : ISeeder
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly IRepository _worldRepository;
    private readonly ITibiaApi _tibiaApi;

    public WorldScanSeeder(ITibiaCharacterFinderDbContext dbContext, IRepository worldRepository, ITibiaApi tibiaApi)
    {
        _dbContext = dbContext;
        _worldRepository = worldRepository;
        _tibiaApi = tibiaApi;
    }

    public async Task Seed()
    {
        if (_dbContext.Database.CanConnect())
        {
            var availableWorlds = await _worldRepository.GetAvailableWorldsAsync();
            foreach (var availableWorld in availableWorlds)
            {
                try
                {
                    var worldScan = await CreateWorldScanAsync(availableWorld);
                    _dbContext.WorldScans.Add(worldScan);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{availableWorld.Name} - cannot deserialized object");
                    Console.WriteLine(e);
                    continue;
                }
            }
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Save success " + DateTime.Now);
        }
        else
        {
            Console.WriteLine("Cannot connect to DB");
        }
    }

    private async Task<WorldScan> CreateWorldScanAsync(World world)
    {
        var charactersOnline = await _tibiaApi.FetchCharactersOnlineFromApi(world.Name);

        var worldScan = new WorldScan
        {
            CharactersOnline = charactersOnline,
            WorldId = world.WorldId,
            ScanCreateDateTime = DateTime.UtcNow,
        };
        return worldScan;
    }
}