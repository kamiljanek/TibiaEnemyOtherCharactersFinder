using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace TibiaCharFinderAPI.Services
{
    public interface IWorldService
    {
        WorldDto GetById(int id);
        IEnumerable<WorldDto> GetAll();
        int Create(CreateWorldDto dto);
        bool Delete(int id);
    }

    public class WorldService : IWorldService
    {
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public WorldService(EnemyCharFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public WorldDto GetById(int id)
        {
            var world = _dbContext.Worlds.Include(w => w.WorldScans).FirstOrDefault(w => w.Id == id);
            if (world is null)
            {
                return null;
            }

            var result = _mapper.Map<WorldDto>(world);
            return result;
        }
        public IEnumerable<WorldDto> GetAll()
        {
            var world = _dbContext.Worlds.Include(w => w.WorldScans).ToList();

            var result = _mapper.Map<List<WorldDto>>(world);
            return result;
        }
        public int Create(CreateWorldDto dto)
        {
            var world = _mapper.Map<World>(dto);
            _dbContext.Worlds.Add(world);
            _dbContext.SaveChanges();

            return world.Id;
        } 
        public bool Delete(int id)
        {
            var world = _dbContext.Worlds.SingleOrDefault(w=>w.Id==id);
            if (world is null)
            {
                return false;
            }
            _dbContext.Worlds.Remove(world);
            _dbContext.SaveChanges();

            return true;
        }
    }
}
