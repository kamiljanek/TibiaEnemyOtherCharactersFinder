using System.ComponentModel.DataAnnotations;
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
    /// <returns>Searched character with details and 10 most scored possible other character names with number of matches.</returns>
    [HttpGet("{characterName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOtherCharacters([FromRoute] [Required] string characterName)
    {
        var result = await _mediator.Send(new GetCharacterWithCorrelationsQuery(characterName));

        return Ok(result);
    }

    /// <summary>
    /// Get a list of character names based on a fragment of the name, sorted in ascending order.
    /// </summary>
    /// <param name="searchText">A fragment of the character name. Required minimum 2 chars length of a fragment name.</param>
    /// <param name="page">The page number to view. Default value = 1</param>
    /// <param name="pageSize">The capacity of a single page. Default value = 10</param>
    /// <returns>A list of character names with total count.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFilteredCharacters(
        [FromQuery] [Required] string searchText,
        [FromQuery] [Required] int page = 1,
        [FromQuery] [Required] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetFilteredCharactersByFragmentNameQuery(searchText, page, pageSize));

        return Ok(result);
    }

    /// <summary>
    /// Get a list of character names starts at fragment of the name, sorted in ascending order.
    /// </summary>
    /// <param name="searchText">A fragment of the character name. Required minimum 2 chars length of a fragment name.</param>
    /// <param name="page">The page number to view. Default value = 1</param>
    /// <param name="pageSize">The capacity of a single page. Default value = 10</param>
    /// <returns>A list of character names.</returns>
    [HttpGet("prompt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFilteredCharactersPrompt(
        [FromQuery] [Required] string searchText,
        [FromQuery] [Required] int page = 1,
        [FromQuery] [Required] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetFilteredCharactersByFragmentNamePromptQuery(searchText, page, pageSize));

        return Ok(result);
    }
}