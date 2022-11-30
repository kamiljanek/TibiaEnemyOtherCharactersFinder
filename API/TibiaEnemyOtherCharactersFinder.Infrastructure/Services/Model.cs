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

        public Task<List<World>> GetAvailableWorldsAsync() => 
            Task.FromResult(_dbContext.Worlds.Where(w => w.IsAvailable).ToList());
        
        public Task<List<World>> GetWorldsAsNoTrackingAsync() => 
            Task.FromResult(_dbContext.Worlds.AsNoTracking().ToList());

        public Task<List<World>> GetAvailableWorldsAsNoTruckingAsync() => 
            Task.FromResult(_dbContext.Worlds.Where(w => w.IsAvailable).AsNoTracking().ToList());
        
        public List<World> GetAvailableWorldsAsNoTrucking() => 
            _dbContext.Worlds.Where(w => w.IsAvailable).AsNoTracking().ToList();

        public Task<List<WorldScan>> GetWorldScansAsync(short worldId) => Task.FromResult(_dbContext.WorldScans
                .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
                .OrderBy(scan => scan.ScanCreateDateTime)
                .ToList());
        
        public Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId) => Task.FromResult(_dbContext.WorldScans
                .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
                .OrderBy(scan => scan.ScanCreateDateTime)
                .Take(2)
                .ToList());
        
        public List<WorldScan> GetFirstTwoWorldScans(short worldId) => _dbContext.WorldScans
                .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
                .OrderBy(scan => scan.ScanCreateDateTime)
                .Take(2)
                .ToList();
    }
}
