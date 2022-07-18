using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Models
{
    public class Model
    {
        private readonly EnemyCharFinderDbContext _dbContext;

        public Model(EnemyCharFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected List<World> GetWorldsFromDbIfIsAvailable()
        {
            return _dbContext.Worlds.Where(w => w.IsAvailable == true).ToList();
        }
    }
}
