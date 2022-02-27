using System;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;

namespace CleanScrapSesionsDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using TibiaArchiveContext context = new TibiaArchiveContext();
            foreach (var item in context.ScrapSesions)
            {
                context.ScrapSesions.Remove(item);
            }
            context.SaveChanges();
        }
    }
}
