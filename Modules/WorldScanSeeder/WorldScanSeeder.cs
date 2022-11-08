using System.Net.Http.Json;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Models;

namespace WorldScanSeeder
{
    public class WorldScanSeeder : Model
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public WorldScanSeeder(TibiaCharacterFinderDbContext dbContext, HttpClient httpClient) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }
        public async Task Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetAvailableWorlds();

                foreach (var world in worlds)
                {
                    try
                    {
                        var worldScan = await CreateWorldScan(world);
                        _dbContext.WorldScans.Add(worldScan);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(world.Name);
                        Console.WriteLine(e);
                        continue;
                    }
                }
                _dbContext.SaveChanges();
            }
        }

        private async Task<WorldScan> CreateWorldScan(World world)
        {
            var charactersOnline = await FetchCharactersOnlineFromApi(world.Name);

            var worldScan = new WorldScan
            {
                CharactersOnline = charactersOnline,
                WorldId = world.WorldId,
                ScanCreateDateTime = DateTime.UtcNow,
                World = world
            };

            return worldScan;
        }

        private async Task<string> FetchCharactersOnlineFromApi(string name)
        {
            var response = await _httpClient.GetAsync($"https://api.tibiadata.com/v3/world/{name}");

            var world = await response.Content.ReadFromJsonAsync<WorldApiResult>();

            var onlinePlayers = world.worlds.world.online_players.Select(x => x.name).ToList();

            return string.Join("|", onlinePlayers);

        }
    }
}

