using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.WebEncoders.Testing;
using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

namespace TibiaCharacterFinderAPI.Controllers
{
    [Route("api/character")]
    public class CharacterController : ControllerBase
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public CharacterController(TibiaCharacterFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// Get as much characters as you wish
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpGet("{amount}")]
        public ActionResult<IEnumerable<CharacterDto>> GetAmount([FromRoute] int amount)
        {
            var characters = _dbContext.Characters
                .Take(amount);
            //.Include(c => c.LogoutWorldCorrelations)
            //.Include(c => c.LoginWorldCorrelations);

            var charactersDto = _mapper.ProjectTo<CharacterDto>(characters);

            return Ok(charactersDto);
        }

        //[HttpGet("name")]
        //public ActionResult<CharacterDto> GetById([FromQuery] int id)
        //{
        //    var character = _dbContext.Characters
        //        .Include(c => c.LogoutWorldCorrelations)
        //        .Include(c => c.LoginWorldCorrelations)
        //        .FirstOrDefault(w => w.Id == id);
        //    var characterDto = _mapper.Map<CharacterDto>(character);



        //    if (characterDto != null)
        //    {
        //        return Ok(characterDto);
        //    }
        //    return NotFound($"No character with id={id}");
        //}

    }
}
