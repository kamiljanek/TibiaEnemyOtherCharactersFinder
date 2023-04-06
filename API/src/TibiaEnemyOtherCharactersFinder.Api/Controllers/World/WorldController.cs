using MediatR;
using Microsoft.AspNetCore.Mvc;
using TibiaEnemyOtherCharactersFinder.Application.Queries.World;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.World;

public class WorldController : WorldBaseController
{
    private readonly IMediator _mediator;

    public WorldController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all active worlds
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActiveWorlds()
    {
        var result = await _mediator.Send(new GetActiveWorldsQuery());

        return Ok(result);
    }
}


