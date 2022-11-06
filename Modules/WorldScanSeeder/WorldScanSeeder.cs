using HtmlAgilityPack.CssSelectors.NetCore;
using Shered.Services;
using System.Net.Http.Json;
using System.Text;
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

                Console.WriteLine(worlds[0].Name);

                foreach (var world in worlds)
                {
                    var worldScan = await CreateWorldScan(world);

                    Console.WriteLine(worldScan.CharactersOnline);

                    _dbContext.WorldScans.Add(worldScan);
                }
                _dbContext.SaveChanges();
            }
        }

        private async Task<WorldScan> CreateWorldScan(World world)
        {
            var charactersOnline = await FetchCharactersOnlineFromApi(world.Name);

            Console.WriteLine(world.Url);
            Console.WriteLine(charactersOnline);

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

        //private string FetchOnlineCharacters(string world)
        //{
        //    _decompressor.Decompress();

        //    var stringBuilder = new StringBuilder();
        //    var names = new List<string>();
        //    var document = _decompressor.web.Load(world);

        //    Console.WriteLine(document.ParsedText);

        //    var items = document.QuerySelectorAll(".Odd [href], .Even [href]");
        //    foreach (var item in items)
        //    {
        //        string name = item.InnerHtml.Replace("&#160;", " ");
        //        names.Add(name);
        //    }
        //    stringBuilder.AppendJoin('|', names);
        //    return stringBuilder.ToString();
        //}
    }
}

