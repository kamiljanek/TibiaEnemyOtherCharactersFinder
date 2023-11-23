using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.World;

public record GetActiveWorldsQuery(bool? Available) : IRequest<GetWorldsResult>;

public class GetActiveWorldsQueryHandler : IRequestHandler<GetActiveWorldsQuery, GetWorldsResult>
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public GetActiveWorldsQueryHandler(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<GetWorldsResult> Handle(GetActiveWorldsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);

        var parameters = new
        {
            Available = request.Available
        };

        var activeWorlds = (await connection.QueryAsync<WorldResult>(GenerateQueries.GetActiveWorlds, parameters)).ToArray();

        var result = new GetWorldsResult() { Worlds = activeWorlds};

        return result;
    }
}
