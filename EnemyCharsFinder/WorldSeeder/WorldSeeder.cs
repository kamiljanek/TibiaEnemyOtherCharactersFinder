using HtmlAgilityPack.CssSelectors.NetCore;
using TibiaCharFinder.Entities;
using Microsoft.EntityFrameworkCore;

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
                var serverNames = GetServerNamesFromTibiaCom();
                foreach (var serverName in serverNames)
                {
                    if (!_dbContext.Worlds.Any(o => o.Name == serverName))
                    {
                        var world = CreateWorld(serverName);
                        _dbContext.Worlds.Add(world);
                    }
                }
                _dbContext.SaveChanges();
            }
        }

        public void CheckIfServerIsAvailable()
        {
                var availableServerNames = GetServerNamesFromTibiaCom();
                var dbServerNames = GetServerNamesFromDb();

        }

        private List<string> GetServerNamesFromDb()
        {
            using (var ctx = new EnemyCharFinderDbContext())
            {
                var studentList = _dbContext.Database
                    .SqlQuery<string>("Select * from Students")
                    .ToList();
            }
            return _dbContext.Database.SqlQuery<string>().ToList();
        }

        private World CreateWorld(string serverName)
        {
            var serverUrl = ServerUrl(serverName);
            var world = new World()
            {
                Name = serverName,
                Url = serverUrl,
                IsAvailable = true
            };
            return world;
        }
        private List<string> GetServerNamesFromTibiaCom()
        {
            List<string> serverNames = new List<string>();
            _decompressor.Decompress();
            var document = _decompressor.web.Load(_mainUrl);
            var tables = document.QuerySelectorAll(".TableContent");
            var items = tables[2].QuerySelectorAll(".Odd, .Even");
            foreach (var item in items)
            {
                var a = item.QuerySelectorAll("a");
                var text = a[0].InnerText;
                serverNames.Add(text);
            }
            return serverNames;
        }
        private string ServerUrl(string serverName)
        {
            return $"{_mainUrl}&world={serverName}";
        }
    }
}
