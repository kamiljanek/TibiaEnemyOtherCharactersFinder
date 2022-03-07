using System;
using System.Diagnostics;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;

namespace AddScrapSesionToDb
{
    public class Program
    {
        static void Main(string[] args)
        {

            var scrapSesionDb = new AddScrapSesion();
            scrapSesionDb.Run();
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();


            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

        }
    }
}
