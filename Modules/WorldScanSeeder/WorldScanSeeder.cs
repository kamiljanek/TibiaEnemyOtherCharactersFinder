using HtmlAgilityPack.CssSelectors.NetCore;
using Shered.Services;
using System.Text;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Models;

namespace WorldScanSeeder
{
    public class WorldScanSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly Decompressor _decompressor;

        public WorldScanSeeder(TibiaCharacterFinderDbContext dbContext, Decompressor decompressor) : base(dbContext)
        {
            _dbContext = dbContext;
            _decompressor = decompressor;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetAvailableWorlds();
                Console.WriteLine(worlds[0].Name);

                foreach (var world in worlds)
                {
                    var worldScan = CreateWorldScan(world);

                    Console.WriteLine(worldScan.CharactersOnline);

                    _dbContext.WorldScans.Add(worldScan);
                }
                _dbContext.SaveChanges();
            }
        }

        private WorldScan CreateWorldScan(World world)
        {
            var charactersOnline = FetchOnlineCharacters(world.Url);
            var worldScan = new WorldScan
            {
                CharactersOnline = charactersOnline,
                WorldId = world.WorldId,
                ScanCreateDateTime = DateTime.UtcNow,
                World = world
            };
            return worldScan;
        }


        private string FetchOnlineCharacters(string world)
        {
            _decompressor.Decompress();

            var stringBuilder = new StringBuilder();
            var names = new List<string>();
            var document = _decompressor.web.Load(world);
            var items = document.QuerySelectorAll(".Odd [href], .Even [href]");
            foreach (var item in items)
            {
                string name = item.InnerHtml.Replace("&#160;", " ");
                names.Add(name);
            }
            stringBuilder.AppendJoin('|', names);
            return stringBuilder.ToString();
        }
    }
}

