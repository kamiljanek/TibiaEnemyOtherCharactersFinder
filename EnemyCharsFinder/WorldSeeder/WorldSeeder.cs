using HtmlAgilityPack.CssSelectors.NetCore;
using TibiaCharFinder.Entities;

namespace WorldSeeder
{
    public class WorldSeeder
    {
        private readonly string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly Decompressor _decompressor;

        public WorldSeeder(EnemyCharFinderDbContext dbContext, Decompressor decompressor)
        {
            _dbContext = dbContext;
            _decompressor = decompressor;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worldNames = GetWorldNamesFromTibiaCom();
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
        }

        public void TurnOffIfWorldIsUnavailable()
        {
            if (_dbContext.Database.CanConnect())
            {
                var availableWorldNames = GetWorldNamesFromTibiaCom();
                var dbWorldNames = GetWorldsFromDb();
                foreach (var dbWorldName in dbWorldNames)
                {
                    if (!availableWorldNames.Contains(dbWorldName.Name))
                    {
                        var worldName = _dbContext.Worlds.First(i => i.Name == dbWorldName.Name);
                        worldName.IsAvailable = false;
                    }
                }
                _dbContext.SaveChanges();
            }
        }

        private List<World> GetWorldsFromDb()
        {
            return _dbContext.Worlds.Where(w => w.Name != null).ToList();
        }

        private World CreateWorld(string worldName)
        {
            var worldUrl = GenerateWorldUrl(worldName);
            var world = new World()
            {
                Name = worldName,
                Url = worldUrl,
                IsAvailable = true
            };
            return world;
        }
        private List<string> GetWorldNamesFromTibiaCom()
        {
            List<string> worldNames = new List<string>();
            _decompressor.Decompress();
            var document = _decompressor.web.Load(_mainUrl);
            var tables = document.QuerySelectorAll(".TableContent");
            var items = tables[2].QuerySelectorAll(".Odd, .Even");
            foreach (var item in items)
            {
                var a = item.QuerySelectorAll("a");
                var text = a[0].InnerText;
                worldNames.Add(text);
            }
            return worldNames;
        }
        private string GenerateWorldUrl(string worldName)
        {
            return $"{_mainUrl}&world={worldName}";
        }
    }
}
