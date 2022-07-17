using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace TibiaCharFinderAPI.Controllers
{
    [Route("api/character")]
    public class CharacterController : ControllerBase
    {
        private readonly EnemyCharFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public CharacterController(EnemyCharFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CharacterDto>> GetExamples()
        {
            List<Character> characters = new List<Character>()
            {
                _dbContext.Characters.Include(c=>c.WorldCorrelations).First(),
                _dbContext.Characters.Skip(1).First(),
                _dbContext.Characters.Skip(4).First(),
                _dbContext.Characters.Skip(16).First(),
            };

            var charactersDto = _mapper.Map<List<CharacterDto>>(characters);

            return Ok(charactersDto);
        }
        [HttpGet("{amount}")]
        public ActionResult<IEnumerable<CharacterDto>> GetAmount([FromRoute] int amount)
        {
            var characters = _dbContext.Characters.Take(amount).Include(c => c.WorldCorrelations);

            var charactersDto = _mapper.Map<List<CharacterDto>>(characters);

            return Ok(charactersDto);
        }
        [HttpGet("name")]
        public ActionResult<CharacterDto> GetById([FromQuery] int id)
        {
            var character = _dbContext.Characters.Include(c=>c.WorldCorrelations).FirstOrDefault(w => w.Id == id);
            var characterDto = _mapper.Map<CharacterDto>(character);

            if (characterDto != null)
            {
                return Ok(characterDto);
            }
            return NotFound($"No character with id={id}");
        }  
    }
}
