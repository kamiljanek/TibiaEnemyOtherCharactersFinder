using System.Net.Http.Json;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Services;

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
                    var worldNames = await GetWorldNamesFromTibiaApi();
                    foreach (var worldName in worldNames)
                    {
                        if (!_dbContext.Worlds.Any(o => o.Name == worldName))
                        {
                            var world = CreateWorld(worldName);
                            _dbContext.Worlds.Add(world);
                        }
                    }
                    _dbContext.SaveChanges();
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

        public async Task TurnOffIfWorldIsUnavailable()
        {
            if (_dbContext.Database.CanConnect())
            {
                try
                {
                    var availableWorldNamesFromTibiaApi = await GetWorldNamesFromTibiaApi();
                    var worldNamesFromDb = GetAvailableWorlds();

                    foreach (var worldNameFromDb in worldNamesFromDb)
                    {
                        if (!availableWorldNamesFromTibiaApi.Contains(worldNameFromDb.Name))
                        {
                            var worldName = _dbContext.Worlds.First(i => i.Name == worldNameFromDb.Name);
                            worldName.IsAvailable = false;
                        }
                    }
                    _dbContext.SaveChanges();
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
}
