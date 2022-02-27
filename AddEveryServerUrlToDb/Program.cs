using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;

using System;

namespace AddEveryServerUrlToDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            decompressor.Servers(Link._mainUrl);

            Link link = new Link();
            link.SetLinks(decompressor.ServersList);

            using TibiaArchiveContext context = new TibiaArchiveContext();
            foreach (var item in context.Urls)
            {
                context.Urls.Remove(item);
            }
            foreach (var item in link.Links)
            {
                Url url = new Url()
                {
                    Adrress = item
                };
                context.Urls.Add(url);
            }

            context.SaveChanges();
        }
    }
}
