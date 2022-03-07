using EnemyCharsFinder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanScrapSesionsDb
{
    public class CleanScrapSesion
    {
        public void Run()
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
