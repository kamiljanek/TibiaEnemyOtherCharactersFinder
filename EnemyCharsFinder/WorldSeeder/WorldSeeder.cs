using HtmlAgilityPack.CssSelectors.NetCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace WorldSeeder
{
    public class WorldSeeder : Model, ISeeder
    {
        private readonly string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly Decompressor _decompressor;

        public WorldSeeder(EnemyCharFinderDbContext dbContext, Decompressor decompressor) : base(dbContext)
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
                var dbWorldNames = GetWorldsFromDbIfIsAvailable();
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

        private World CreateWorld(string worldName)
        {
            var worldUrl = GetWorldUrl(worldName);
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
        private string GetWorldUrl(string worldName)
        {
            return $"{_mainUrl}&world={worldName}";
        }
    }
}
