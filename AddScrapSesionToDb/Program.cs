using System;
using System.Diagnostics;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;

namespace AddScrapSesionToDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            StringBuilder builder = new StringBuilder();

            using TibiaArchiveContext context = new TibiaArchiveContext();
            var urls = context.Urls;
            foreach (var item in urls)
            {
                decompressor.Names(item.Adrress, builder);
            }
            var scrapSesion = builder.ToString();
            ScrapSesion scrap = new ScrapSesion()
            {
                DatePublished = DateTime.Now,
                Names = scrapSesion
            };
            context.ScrapSesions.Add(scrap);
            context.SaveChanges();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

        }
    }
}
