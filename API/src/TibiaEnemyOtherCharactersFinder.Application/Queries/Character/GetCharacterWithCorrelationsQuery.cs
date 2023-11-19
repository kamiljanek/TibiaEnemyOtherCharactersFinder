using Dapper;
using FluentValidation.Results;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Validations;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetCharacterWithCorrelationsQuery(string Name) : IRequest<CharacterWithCorrelationsResult>;

public class GetCharacterWithCorrelationsQueryHandler : IRequestHandler<GetCharacterWithCorrelationsQuery, CharacterWithCorrelationsResult>
{
    private readonly IDapperConnectionProvider _connectionProvider;
    private readonly ITibiaDataClient _tibiaDataClient;
    private readonly IRequestValidator _validator;

    public GetCharacterWithCorrelationsQueryHandler(IDapperConnectionProvider connectionProvider, ITibiaDataClient tibiaDataClient, IRequestValidator validator)
    {
        _connectionProvider = connectionProvider;
        _tibiaDataClient = tibiaDataClient;
        _validator = validator;
    }

    public async Task<CharacterWithCorrelationsResult> Handle(GetCharacterWithCorrelationsQuery request, CancellationToken cancellationToken)
    {
        _validator.ValidSearchTextLenght(request.Name);
        _validator.ValidSearchTextCharacters(request.Name);

        var character = await _tibiaDataClient.FetchCharacter(request.Name);
        if (character is null)
        {
            throw new TibiaDataApiConnectionException();
        }
        if (string.IsNullOrWhiteSpace(character.characters.character.name))
        {
            throw new NotFoundException(nameof(Character), request.Name);
        }

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            CharacterName = character.characters.character.name.ToLower()
        };

        var correlations = await connection.QueryAsync<CorrelationResult>(GenerateQueries.GetOtherPossibleCharacters, parameters);
        var result = new CharacterWithCorrelationsResult
        {
            FormerNames = character.characters.character.former_names ?? Array.Empty<string>(),
            FormerWorlds = character.characters.character.former_worlds ?? Array.Empty<string>(),
            Name = character.characters.character.name,
            Level = character.characters.character.level,
            Traded = character.characters.character.traded,
            Vocation = character.characters.character.vocation,
            World = character.characters.character.world,
            LastLogin = character.characters.character.last_login,
            OtherVisibleCharacters = character.characters.other_characters is null ? Array.Empty<string>() : character.characters.other_characters.Select(ch => ch.name).ToList(),
            PossibleInvisibleCharacters = correlations is null ? Array.Empty<CorrelationResult>() : correlations.ToList()
        };

        return result;
    }
}
