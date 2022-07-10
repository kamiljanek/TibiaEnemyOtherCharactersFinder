using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinder.Entities;

namespace WorldSeeder
{
    public class WorldSeeder
    {
        private readonly string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";
        private readonly EnemyCharFinderDbContext _dbContext;

        public WorldSeeder(EnemyCharFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {

        }
        public void Servers()
        {
            var document = web.Load(_mainUrl);
            var tables = document.QuerySelectorAll(".TableContent");
            var items = tables[2].QuerySelectorAll(".Odd, .Even");
            foreach (var item in items)
            {
                var a = item.QuerySelectorAll("a");
                var text = a[0].InnerText;
                ServersList.Add(text);
            }
        }
        public void SetLinks(List<string> serverNames)
        {
            List<string> links = new List<string>();
            foreach (var serverName in serverNames)
            {
                var link = $"https://www.tibia.com/community/?subtopic=worlds&world={serverName}";
                links.Add(link);
            }
            Links = links;
        }
    }
}
