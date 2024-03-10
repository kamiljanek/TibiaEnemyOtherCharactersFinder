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

        var character = await _tibiaDataClient.FetchCharacter(request.Name, false);
        if (character is null)
        {
            throw new TibiaDataApiConnectionException();
        }
        if (string.IsNullOrWhiteSpace(character.Name))
        {
            throw new NotFoundException(nameof(Character), request.Name);
        }

        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        var parameters = new
        {
            CharacterName = character.Name.ToLower()
        };

        var correlations = (await connection.QueryAsync<CorrelationResult>(GenerateQueries.GetOtherPossibleCharacters, parameters)).ToArray();
        var result = new CharacterWithCorrelationsResult
        {
            FormerNames = character.FormerNames ?? Array.Empty<string>(),
            FormerWorlds = character.FormerWorlds ?? Array.Empty<string>(),
            Name = character.Name,
            Level = character.Level,
            Traded = character.Traded,
            Vocation = character.Vocation,
            World = character.World,
            LastLogin = character.LastLogin,
            OtherVisibleCharacters = character.OtherCharacters ?? Array.Empty<string>(),
            PossibleInvisibleCharacters = correlations
        };

        return result;
    }
}
