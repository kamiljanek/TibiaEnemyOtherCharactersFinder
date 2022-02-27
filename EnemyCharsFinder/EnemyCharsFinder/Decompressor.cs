using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EnemyCharsFinder
{
    public class Decompressor
    {
        public HtmlWeb web = new HtmlWeb();
        public List<string> NameList = new List<string>();
        public List<string> ServersList = new List<string>();
        public void Decompress()
        {
            web.PreRequest += (HttpWebRequest request) =>
            {
                request.AutomaticDecompression = DecompressionMethods.All;
                return true;
            };
        }
        public void Names(string url)
        {
            var document = web.Load(url);
            var items = document.QuerySelectorAll(".Odd, .Even");
            foreach (var item in items)
            {
                var tds = item.QuerySelectorAll("td");
                var text = tds[0].InnerText;
                NameList.Add(text);
            }
        }
        public void Servers(string url)
        {
            var document = web.Load(url);
            var tables = document.QuerySelectorAll(".TableContent");
            var items = tables[2].QuerySelectorAll(".Odd, .Even");
            foreach (var item in items)
            {
                var a = item.QuerySelectorAll("a");
                var text = a[0].InnerText;
                ServersList.Add(text);
            }
        }


    }
}
