using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.Services;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.Character;

public record GetCharacterWithCorrelationsQuery(string Name) : IRequest<CharacterWithCorrelationsResult>;

public class GetCharacterWithCorrelationsQueryHandler : IRequestHandler<GetCharacterWithCorrelationsQuery, CharacterWithCorrelationsResult>
{
    private readonly IDapperConnectionProvider _connectionProvider;
    private readonly ITibiaDataService _tibiaDataService;

    public GetCharacterWithCorrelationsQueryHandler(IDapperConnectionProvider connectionProvider, ITibiaDataService tibiaDataService)
    {
        _connectionProvider = connectionProvider;
        _tibiaDataService = tibiaDataService;
    }

    public async Task<CharacterWithCorrelationsResult> Handle(GetCharacterWithCorrelationsQuery request, CancellationToken cancellationToken)
    {
        var parameters = new { CharacterName = request.Name.ToLower() };
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var character = await _tibiaDataService.FetchCharacter(request.Name);
        if (string.IsNullOrWhiteSpace(character.characters.character.name))
        {
            return null;
        }

        var correlations = await connection.QueryAsync<CorrelationResult>(GenerateQueries.NpgsqlGetOtherPossibleCharacters, parameters);
        var result = new CharacterWithCorrelationsResult
        {
            FormerNames = character.characters.character.former_names,
            FormerWorlds = character.characters.character.former_worlds,
            Name = character.characters.character.name,
            Level = character.characters.character.level,
            Traded = character.characters.character.traded,
            Vocation = character.characters.character.vocation,
            World = character.characters.character.world,
            LastLogin = character.characters.character.last_login,
            OtherVisibleCharacters = character.characters.other_characters.Select(ch => ch.name).ToList(),
            PossibleInvisibleCharacters = correlations.ToList()
        };

        return result;
    }
}
