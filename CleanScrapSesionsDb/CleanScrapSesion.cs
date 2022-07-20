using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace CleanScrapSesionsDb
{
    public class CleanScrapSesion : Model
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public CleanScrapSesion(TibiaCharacterFinderDbContext dbContext) : base(dbContext)
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
