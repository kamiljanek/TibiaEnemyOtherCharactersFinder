using Dapper;
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
        if (string.IsNullOrWhiteSpace(character.Character?.Character?.Name))
        {
            throw new NotFoundException(nameof(Character), request.Name);
        }

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            CharacterName = character.Character.Character.Name.ToLower()
        };

        var correlations = await connection.QueryAsync<CorrelationResult>(GenerateQueries.GetOtherPossibleCharacters, parameters);
        var result = new CharacterWithCorrelationsResult
        {
            FormerNames = character.Character.Character.FormerNames ?? Array.Empty<string>(),
            FormerWorlds = character.Character.Character.FormerWorlds ?? Array.Empty<string>(),
            Name = character.Character.Character.Name,
            Level = character.Character.Character.Level,
            Traded = character.Character.Character.Traded,
            Vocation = character.Character.Character.Vocation,
            World = character.Character.Character.World,
            LastLogin = character.Character.Character.LastLogin,
            OtherVisibleCharacters = character.Character.OtherCharacters is null ? Array.Empty<string>() : character.Character.OtherCharacters.Select(ch => ch.Name).ToList(),
            PossibleInvisibleCharacters = correlations is null ? Array.Empty<CorrelationResult>() : correlations.ToList()
        };

        return result;
    }
}
