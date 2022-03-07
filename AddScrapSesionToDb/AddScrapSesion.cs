using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddScrapSesionToDb
{
    public class AddScrapSesion
    {
        public void Run()
        {
            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            StringBuilder builder = new StringBuilder();

            using TibiaArchiveContext context = new TibiaArchiveContext();
            var urls = context.Urls;
            foreach (var item in urls)
            {
                decompressor.Names(item.Adrress, builder);
                var scrapSesion = builder.ToString();
                ScrapSesion scrap = new ScrapSesion()
                {
                    DatePublished = DateTime.Now,
                    OnlineCharacterNames = scrapSesion,
                    ServerName = item.Adrress.Replace("https://www.tibia.com/community/?subtopic=worlds&world=", null)
                };
                context.ScrapSesions.Add(scrap);
            }
            context.SaveChanges();

            Console.WriteLine(DateTime.Now);

        }
    }
}
