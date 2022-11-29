using Newtonsoft.Json;
using System.IO.Compression;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

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
                var availableWorlds = await GetAvailableWorldsAsync();
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
                _dbContext.SaveChanges();
                Console.WriteLine("Save success " + DateTime.Now);
            }
            else
            {
                Console.WriteLine("Cannot connect to DB");
            }
        }

        private async Task<WorldScan> CreateWorldScanAsync(World world)
        {
            var charactersOnline = await FetchCharactersOnlineFromApi(world.Name);

            var worldScan = new WorldScan
            {
                CharactersOnline = charactersOnline,
                WorldId = world.WorldId,
                ScanCreateDateTime = DateTime.UtcNow,
            };
            return worldScan;
        }

        private async Task<string> FetchCharactersOnlineFromApi(string name)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.tibiadata.com/v3/world/{name}");

            using var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string content = await ReadContentAsString(response);

                var contentDeserialized = JsonConvert.DeserializeObject<TibiaApiWorldResult>(content);

                var onlinePlayers = contentDeserialized.worlds.world.online_players.Select(x => x.name).ToList();

                Console.WriteLine($"{name} - successfull");

                return string.Join("|", onlinePlayers);
            }
            Console.WriteLine($"{name} - code isn't success");

            return string.Empty;
        }

        private async Task<string> ReadContentAsString(HttpResponseMessage response)
        {
            // Check whether response is compressed
            if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
            {
                // Decompress manually
                using (var s = await response.Content.ReadAsStreamAsync())
                {
                    using (var decompressed = new GZipStream(s, CompressionMode.Decompress))
                    {
                        using (var rdr = new StreamReader(decompressed))
                        {
                            return await rdr.ReadToEndAsync();
                        }
                    }
                }
            }
            // Use standard implementation if not compressed
            return await response.Content.ReadAsStringAsync();
        }
    }
}

