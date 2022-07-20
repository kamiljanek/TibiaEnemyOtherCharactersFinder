using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace TibiaCharFinderAPI.Controllers
{
    [Route("api/correlation")]
    public class WorldCorrelationController : ControllerBase
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IMapper _mapper;

        public WorldCorrelationController(TibiaCharacterFinderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("{amount}")]
        public ActionResult<IEnumerable<WorldCorrelationDto>> GetAmount([FromRoute] int amount)
        {
            var worldCorrelations = _dbContext.WorldCorrelations
                .Take(amount)
                .Include(e => e.LoginCharacter)
                .Include(e => e.LogoutCharacter);

            var worldCorrelationsDto = _mapper.Map<List<WorldCorrelationDto>>(worldCorrelations);

            return Ok(worldCorrelationsDto);
        }
    }
}
