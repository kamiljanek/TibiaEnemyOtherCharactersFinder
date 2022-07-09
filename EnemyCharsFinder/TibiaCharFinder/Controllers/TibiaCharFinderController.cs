using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TibiaCharFinder.Entities;

namespace TibiaCharFinder.Controllers
{
    [Route("api/character")]
    public class TibiaCharFinderController : ControllerBase
    {
        private readonly EnemyCharFinderDbContext _dbContext;
        public TibiaCharFinderController(EnemyCharFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WorldScan>> GetAll()
        {
            var scanWorldList = _dbContext.WorldScans.ToList();
            return Ok(scanWorldList);
        }

        [HttpGet ("{id}")]
        public ActionResult<WorldScan> Get([FromRoute] int id)
        {
            var scanWorld = _dbContext.WorldScans.FirstOrDefault(w => w.Id == id);
            if (scanWorld != null)
            {
                return Ok(scanWorld);
            }
            return NotFound();
        }
        [HttpGet("name/{name}")]
        public ActionResult<Character> Get([FromRoute] string name)
        {
            var character = _dbContext.Characters.FirstOrDefault(w => w.Name == name);
            if (character != null)
            {
                return Ok(character);
            }
            return NotFound();
        }
    }
}
