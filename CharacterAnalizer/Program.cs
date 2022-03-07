using EnemyCharsFinder.Data;
using System;

namespace CharacterAnalizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Analyzer();
        }
        public static void Analyzer()
        {
            using TibiaArchiveContext context = new TibiaArchiveContext();
            var scrapSesion = context.ScrapSesions;
            for (int i = 0; i < scrapSesion.Count(); i++)
            {
                var firstScrapSesion = scrapSesion.Skip(i).First();
                var secondScrapSesion = scrapSesion.Skip(i+1).First();
                Console.WriteLine(firstScrapSesion.Id);
                Console.WriteLine(secondScrapSesion.Id);

                //if (firstScrapSesion.OnlineCharacterNamesNames == secondScrapSesion.OnlineCharacterNames)
                //{
                //    Console.WriteLine("are the same");
                //    context.ScrapSesions.Remove(firstScrapSesion);
                //    context.SaveChanges();
                //    //Analyzer();
                //}
                //else
                //{
                //    Console.WriteLine("are not the same");
                //}
            }

            //foreach (var item in context.ScrapSesions)
            //{
            //    context.ScrapSesions.Remove(item);
            //}
        }
    }
}
