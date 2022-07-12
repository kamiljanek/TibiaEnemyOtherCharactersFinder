using EnemyCharsFinder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinder.Entities;

namespace CleanScrapSesionsDb
{
    public class CleanScrapSesion
    {
        public void Run()
        {
            using EnemyCharFinderDbContext context = new EnemyCharFinderDbContext();
            foreach (var item in context.WorldScans)
            {
                context.WorldScans.Remove(item);
            }
            context.SaveChanges();
        }
    }
}
