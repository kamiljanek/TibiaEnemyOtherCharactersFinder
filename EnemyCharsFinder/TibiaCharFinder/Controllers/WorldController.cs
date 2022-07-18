using Microsoft.AspNetCore.Mvc;
using TibiaCharFinderAPI.Models;
using TibiaCharFinderAPI.Services;

namespace TibiaCharFinderAPI.Controllers
{
    [Route("api/world")]
    public class WorldController : ControllerBase
    {
        private readonly IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        [HttpGet("{id}")]
        public ActionResult<WorldDto> GetById([FromRoute] int id)
        {
            var worldDto = _worldService.GetById(id);

            if (worldDto != null)
            {
                return Ok(worldDto);
            }
            return NotFound($"No world with id={id}");
        }
        [HttpGet()]
        public ActionResult<WorldDto> GetAll()
        {
            var worldDtos = _worldService.GetAll();

            return Ok(worldDtos);
        }

        [HttpPost]
        public ActionResult Create([FromBody] CreateWorldDto dto)
        {
            var id = _worldService.Create(dto);
            return Created($"api/world/{id}", null);
        } 
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var isDeleted = _worldService.Delete(id);
           
            if (isDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}

