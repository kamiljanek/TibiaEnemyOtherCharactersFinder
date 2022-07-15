using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Controllers
{
    [Route("api/character")]
    public class CharacterController : ControllerBase
    {
        private readonly EnemyCharFinderDbContext _dbContext;

        public CharacterController(EnemyCharFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Character>> GetExamples()
        {
            List<Character> characters = new List<Character>()
            {
                _dbContext.Characters.First(),
                _dbContext.Characters.Skip(1).First(),
                _dbContext.Characters.Skip(4).First(),
                _dbContext.Characters.Skip(16).First(),
            };
            return Ok(characters);
        }
        [HttpGet("{amount}")]
        public ActionResult<IEnumerable<Character>> GetFew([FromRoute] int amount)
        {
            var characters = _dbContext.Characters.Take(amount);
            return Ok(characters);
        }
        [HttpGet("id/{id}")]
        public ActionResult<Character> GetFromId([FromRoute] int id)
        {
            var character = _dbContext.Characters.FirstOrDefault(w => w.Id == id);
            if (character != null)
            {
                return Ok(character);
            }
            return NotFound();
        }
    }
}
