using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyCharsFinder
{
    public class Link
    {
        public static string _mainUrl = "https://www.tibia.com/community/?subtopic=worlds";
        public List<string>? Links { get; set; }
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
