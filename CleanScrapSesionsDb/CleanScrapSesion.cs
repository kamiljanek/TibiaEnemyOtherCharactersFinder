using Microsoft.EntityFrameworkCore;
using TibiaCharFinder.Entities;
using TibiaCharFinder.Models;

namespace CleanScrapSesionsDb
{
    public class CleanScrapSesion : Model
    {
        private readonly EnemyCharFinderDbContext _dbContext;

        public CleanScrapSesion(EnemyCharFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Clean<T>(DbSet<T> obj) where T : class
        {
            foreach (var item in obj)
            {
                obj.Remove(item);
            }
            _dbContext.SaveChanges();
        } 
       
    }
}
