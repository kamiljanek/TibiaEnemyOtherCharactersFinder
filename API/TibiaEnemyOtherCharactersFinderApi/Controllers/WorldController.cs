using Microsoft.AspNetCore.Mvc;
using TibiaEnemyOtherCharactersFinderApi.Models;
using TibiaEnemyOtherCharactersFinderApi.Services;

namespace TibiaEnemyOtherCharactersFinderApi.Controllers
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

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateWorldDto dto, [FromRoute] int id)
        {
            var isUpdated = _worldService.Update(dto, id);
            if (isUpdated)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}

