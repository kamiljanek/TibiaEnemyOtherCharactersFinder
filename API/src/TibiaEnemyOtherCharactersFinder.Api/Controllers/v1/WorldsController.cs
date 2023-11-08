using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TibiaEnemyOtherCharactersFinder.Application.Queries.World;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.v1;

[ApiVersion("1.0")]
public class WorldsController : TibiaBaseController
{
    private readonly IMediator _mediator;

    public WorldsController(IMediator mediator)
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


