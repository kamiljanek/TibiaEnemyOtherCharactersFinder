using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace WorldSeeder;

public class WorldSeederService : IWorldSeederService
{
    private const string MainUrl = "https://www.tibia.com/community/?subtopic=worlds";

    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly ITibiaDataClient _tibiaDataClient;

    private IReadOnlyList<string> _worldsNamesFromTibiaDataProvider;
    private List<World> _worldsFromDb;

    public WorldSeederService(ITibiaCharacterFinderDbContext dbContext, ITibiaDataClient tibiaDataClient)
    {
        _dbContext = dbContext;
        _tibiaDataClient = tibiaDataClient;
    }

    public async Task SetProperties()
    {
        _worldsNamesFromTibiaDataProvider = await _tibiaDataClient.FetchWorldsNames();
        _worldsFromDb = await _dbContext.Worlds.AsNoTracking().ToListAsync();
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

            await _dbContext.Worlds.AddRangeAsync(newWorlds);
            await _dbContext.SaveChangesAsync();
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
            var unavailableWorlds = _worldsFromDb
                .Where(world => !_worldsNamesFromTibiaDataProvider.Contains(world.Name))
            .Select(world => { world.IsAvailable = false; return world; }).ToList();

            foreach (var world in unavailableWorlds)
            {
                await _dbContext.Worlds
                    .Where(w => w.WorldId == world.WorldId)
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(w => w.IsAvailable, world.IsAvailable));
            }
    }

    public async Task TurnOnIfWorldIsAvailable()
    {
            var worldsToTurnOn = _worldsFromDb
                .Where(world => _worldsNamesFromTibiaDataProvider.Contains(world.Name) && !world.IsAvailable);

            foreach (var world in worldsToTurnOn)
            {
                await _dbContext.Worlds
                    .Where(w => w.WorldId == world.WorldId)
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(w => w.IsAvailable, true));
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
        return $"{MainUrl}&world={worldName}";
    }
}