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
        public void Clean()
        {
            foreach (var item in _dbContext.WorldCorrelations)
            {
                _dbContext.WorldCorrelations.Remove(item);
            }
            _dbContext.SaveChanges();
        }
    }
}
