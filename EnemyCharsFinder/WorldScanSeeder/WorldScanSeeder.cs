using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace WorldScanSeeder
{
    public class WorldScanSeeder : Model, ISeeder
    {
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly Decompressor _decompressor;

        public WorldScanSeeder(EnemyCharFinderDbContext dbContext, Decompressor decompressor) : base(dbContext)
        {
            _dbContext = dbContext;
            _decompressor = decompressor;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetWorldsFromDbIfIsAvailable();
                foreach (var world in worlds)
                {
                    var worldScan = CreateWorldScan(world);
                    _dbContext.WorldScans.Add(worldScan);
                }
                _dbContext.SaveChanges();
            }
        }

        private WorldScan CreateWorldScan(World world)
        {
            var charactersOnline = GetCharactersOnline(world.Url);
            var worldScan = new WorldScan
            {
                CharactersOnline = charactersOnline,
                WorldId = world.Id,
                ScanCreateDateTime = DateTime.Now,
                World = world
            };
            return worldScan;
        }


        private string GetCharactersOnline(string world)
        {
            _decompressor.Decompress();

            var stringBuilder = new StringBuilder();
            var document = _decompressor.web.Load(world);
            var items = document.QuerySelectorAll(".Odd [href], .Even [href]");
            foreach (var item in items)
            {
                stringBuilder.AppendLine($"{item.InnerText}");
            }
            return stringBuilder.ToString();
        }
    }
}

