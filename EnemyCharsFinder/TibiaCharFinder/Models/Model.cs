using TibiaCharFinder.Entities;

namespace TibiaCharFinder.Models
{
    public class Model
    {
        private readonly EnemyCharFinderDbContext _dbContext;

        public Model(EnemyCharFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected List<World> GetWorldsFromDb()
        {
            return _dbContext.Worlds.Where(w => w.Name != null).ToList();
        }
    }
}
