using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Models;

namespace TibiaEnemyOtherCharactersFinder.Api.Services
{
    public interface IWorldService
    {
        WorldDto GetById(int id);
        IEnumerable<WorldDto> GetAll();
        int Create(CreateWorldDto dto);
        bool Delete(int id);
        bool Update(UpdateWorldDto dto, int id);
    }

    public class WorldService : IWorldService
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public WorldService(TibiaCharacterFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public WorldDto GetById(int id)
        {
            var world = _dbContext.Worlds.Include(w => w.WorldScans).FirstOrDefault(w => w.WorldId == id);
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

            return world.WorldId;
        }
        public bool Delete(int id)
        {
            var world = _dbContext.Worlds.FirstOrDefault(w => w.WorldId == id);
            if (world is null)
            {
                return false;
            }
            _dbContext.Worlds.Remove(world);
            _dbContext.SaveChanges();

            return true;
        }

        public bool Update(UpdateWorldDto dto, int id)
        {
            var world = _dbContext.Worlds.FirstOrDefault(e => e.WorldId == id);
            if (world is null)
            {
                return false;
            }

            world.Name = dto.Name;
            world.Url = dto.Url;
            world.IsAvailable = dto.IsAvailable;

            _dbContext.SaveChanges();

            return true;

        }
    }
}
