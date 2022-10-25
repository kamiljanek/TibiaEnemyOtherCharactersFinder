using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.WebEncoders.Testing;
using TibiaEnemyOtherCharactersFinderApi.Dtos;
using TibiaEnemyOtherCharactersFinderApi.Entities;
using TibiaEnemyOtherCharactersFinderApi.Providers;
using TibiaEnemyOtherCharactersFinderApi.Queries.Character;

namespace TibiaEnemyOtherCharactersFinderApi.Controllers.Character
{
    public class CharacterController : CharacterBaseController
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDapperConnectionProvider _connectionProvider;

        public CharacterController(TibiaCharacterFinderDbContext dbContext, IMapper mapper, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }
        /// <summary>
        /// Get as much characters as you wish
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpGet("{amount}")]
        public ActionResult<IEnumerable<CharacterResult>> GetAmount([FromRoute] int amount)
        {
   
            var characters = _dbContext.Characters
                .Take(amount);

            var charactersDto = _mapper.ProjectTo<CharacterResult>(characters);

            return Ok(charactersDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetCharacter()
        {
            var query = new GetCharacterQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
