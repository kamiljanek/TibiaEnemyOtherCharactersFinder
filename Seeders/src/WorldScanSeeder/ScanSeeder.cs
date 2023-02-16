using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public class ScanSeeder : IScanSeeder
{
    private readonly IRepository _repository;
    private readonly ITibiaApi _tibiaApi;

    private List<World> _availableWorlds;

    public List<World> AvailableWorlds => _availableWorlds;

    public ScanSeeder(IRepository repository, ITibiaApi tibiaApi)
    {
        _repository = repository;
        _tibiaApi = tibiaApi;
    }

    public async Task SetProperties()
    {
        try
        {
            _availableWorlds = await _repository.GetAvailableWorldsAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot Get Available Worlds");
            Console.WriteLine(e);
        }
    }

    public async Task Seed(World availableWorld)
    {
        try
        {
            var worldScan = await CreateWorldScanAsync(availableWorld);
            await _repository.AddAsync(worldScan);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{availableWorld.Name} - cannot deserialized object");
            Console.WriteLine(e);
        }
        Console.WriteLine("Save success " + DateTime.Now);
    }

    private async Task<WorldScan> CreateWorldScanAsync(World world)
    {
        var charactersOnline = await _tibiaApi.FetchCharactersOnlineFromTibiaApi(world.Name);

        return new WorldScan
        {
            CharactersOnline = charactersOnline.ToLower(),
            WorldId = world.WorldId,
            ScanCreateDateTime = DateTime.UtcNow,
        };
    }
}