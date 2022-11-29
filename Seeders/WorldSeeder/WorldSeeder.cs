using System.Net.Http.Json;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace WorldSeeder
{
    public class WorldSeeder : Model
    {
        private const string _getWorldsUrl = "https://api.tibiadata.com/v3/worlds";
        private const string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public WorldSeeder(TibiaCharacterFinderDbContext dbContext, HttpClient httpClient) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }

        public async Task Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                try
                {
                    var worldNamesFromApi = await GetWorldNamesFromTibiaApi();
                    var worldsFromDb = await GetWorldsAsNoTrackingAsync();

                    var newWorlds = worldNamesFromApi
                        .Where(name => !worldsFromDb.Select(world => world.Name).Contains(name))
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
                    var availableWorldNamesFromApi = await GetWorldNamesFromTibiaApi();
                    var worldsFromDb = await GetAvailableWorldsAsync();

                    worldsFromDb
                        .Where(world => !availableWorldNamesFromApi.Contains(world.Name))
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

        private async Task<List<string>> GetWorldNamesFromTibiaApi()
        {
            var response = await _httpClient.GetAsync(_getWorldsUrl);

            var result = await response.Content.ReadFromJsonAsync<TibiaApiWorldsResult>();

            return result.worlds.regular_worlds.Select(world => world.name).ToList();
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
}
