using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;
using System;
using System.Linq;

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
            var characterNamesSesions = new List<ScrapSesion>();
            var characterNamesSesionsa = new List<string>();

            var blogs = scrapSesion.Where(s => s.ServerName == "Adra").ToList();


          

            Console.WriteLine(blogs[0].OnlineCharacterNames);
            //for (int i = 0; i < scrapSesion.Count(); i++)
            //{
            //    var firstScrapSesion = scrapSesion.Skip(i).First();
            //    var secondScrapSesion = scrapSesion.Skip(i+1).First();
            //    Console.WriteLine(firstScrapSesion.Id);
            //    Console.WriteLine(secondScrapSesion.Id);

            //    //if (firstScrapSesion.OnlineCharacterNamesNames == secondScrapSesion.OnlineCharacterNames)
            //    //{
            //    //    Console.WriteLine("are the same");
            //    //    context.ScrapSesions.Remove(firstScrapSesion);
            //    //    context.SaveChanges();
            //    //    //Analyzer();
            //    //}
            //    //else
            //    //{
            //    //    Console.WriteLine("are not the same");
            //    //}
            //}

            //foreach (var item in context.ScrapSesions)
            //{
            //    context.ScrapSesions.Remove(item);
            //}
        }
    }
}
