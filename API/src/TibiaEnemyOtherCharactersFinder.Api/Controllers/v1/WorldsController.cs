﻿using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    /// <returns>Worlds count, names, urls and availability.</returns>
    [HttpGet]
    public async Task<IActionResult> GetActiveWorlds([FromQuery] bool? available)
    {
        var result = await _mediator.Send(new GetActiveWorldsQuery(available));
        return Ok(result);
    }
}