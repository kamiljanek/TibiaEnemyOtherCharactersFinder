using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Application.Services
{
    public class Model
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public Model(TibiaCharacterFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected List<World> GetAvailableWorlds()
        {
            return _dbContext.Worlds.Where(w => w.IsAvailable).ToList();
        }
        
        protected List<World> GetAvailableWorldsAsNoTrucking()
        {
            return _dbContext.Worlds.Where(w => w.IsAvailable).AsNoTracking().ToList();
        }
        
        protected List<World> GetAvailableWorldsIncludingScans()
        {
            return _dbContext.Worlds.Where(w => w.IsAvailable).Include(world => world.WorldScans.OrderBy(scan => scan.ScanCreateDateTime)).ToList();
        }
    }
}
