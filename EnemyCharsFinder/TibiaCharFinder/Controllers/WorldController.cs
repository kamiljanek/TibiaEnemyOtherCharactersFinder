using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace TibiaCharFinderAPI.Controllers
{
    [Route("api/world")]
    public class WorldController : ControllerBase
    {
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public WorldController(EnemyCharFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public ActionResult<WorldDto> GetById([FromRoute] int id)
        {
            var world = _dbContext.Worlds.Include(w=>w.WorldScans).FirstOrDefault(w => w.Id == id);
            var worldDto = _mapper.Map<WorldDto>(world);

            if (worldDto != null)
            {
                return Ok(worldDto);
            }
            return NotFound($"No world with id={id}");
        }

        [HttpPost]
        public ActionResult CreateWorld([FromBody] CreateWorldDto dto)
        {
            var world = _mapper.Map<World>(dto);
            _dbContext.Worlds.Add(world);
            _dbContext.SaveChanges();
            return Created($"api/world/{world.Id}", null);
        }
    }
}
