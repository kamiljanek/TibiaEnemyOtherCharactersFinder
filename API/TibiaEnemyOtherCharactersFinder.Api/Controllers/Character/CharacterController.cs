using MediatR;
using Microsoft.AspNetCore.Mvc;
using TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.Character;

public class CharacterController : CharacterBaseController
{
    private readonly IMediator _mediator;

    public CharacterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get 10 most scores enemy characters
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    [HttpGet("{characterName}")]
    public async Task<IActionResult> GetCharacter([FromRoute] string characterName)
    {
        var result = await _mediator.Send(new GetCharacterWithCorrelationsQuery(characterName));
        return Ok(result);
    }
}