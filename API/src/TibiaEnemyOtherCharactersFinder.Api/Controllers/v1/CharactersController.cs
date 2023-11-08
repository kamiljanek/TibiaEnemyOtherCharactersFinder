using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.v1;

[ApiVersion("1.0")]
public class CharactersController : TibiaBaseController
{
    private readonly IMediator _mediator;

    public CharactersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get character details with 10 most scores possible other character names.
    /// </summary>
    /// <param name="characterName">Name of searched character</param>
    [HttpGet("{characterName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOtherCharacters([FromRoute] string characterName)
    {
        var result = await _mediator.Send(new GetCharacterWithCorrelationsQuery(characterName));
        if (result is null)
        {
            return NotFound("Character does not exist");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a list of character names based on a fragment of the name, sorted in ascending order.
    /// </summary>
    /// <param name="searchText">A fragment of the character name. Required minimum 2 chars length of a fragment name.</param>
    /// <param name="page">The page number to view.</param>
    /// <param name="pageSize">The capacity of a single page.</param>
    /// <param name="searchInMiddle">An option that allows searching within the "CharacterName" property.
    /// By default, it is set to "false," which means the function will return only names that start with the <paramref name="searchText"/>.
    /// Set to "true" to search for names containing the <paramref name="searchText"/> in the middle.</param>
    /// <returns>A list of character names.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFilteredCharacters(
        [FromQuery] string searchText,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] bool searchInMiddle = false)
    {
        var result = await _mediator.Send(new GetFilteredCharacterListByFragmentNameQuery(searchText, page, pageSize, searchInMiddle));
        if (result.Count == 0)
        {
            return NotFound("Character does not exist");
        }

        return Ok(result);
    }
}