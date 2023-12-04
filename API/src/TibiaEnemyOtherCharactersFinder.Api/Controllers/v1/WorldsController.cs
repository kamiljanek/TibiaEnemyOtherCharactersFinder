using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    /// Get filtered worlds.
    /// </summary>
    /// <param name="available">If "true" than return all active worlds at this moment in applocation.</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Worlds count, names, urls and availability.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorlds([FromQuery] bool? available, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetActiveWorldsQuery(available), ct);
        return Ok(result);
    }
}
