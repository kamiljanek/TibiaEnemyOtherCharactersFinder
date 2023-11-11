using Dapper;
using MediatR;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Application.Queries.World;

public record GetActiveWorldsQuery(bool? Available) : IRequest<GetActiveWorldsResult>;

public class GetActiveWorldsQueryHandler : IRequestHandler<GetActiveWorldsQuery, GetActiveWorldsResult>
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public GetActiveWorldsQueryHandler(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<GetActiveWorldsResult> Handle(GetActiveWorldsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);

        var parameters = new
        {
            Available = request.Available
        };

        var activeWorlds = (await connection.QueryAsync<ActiveWorldResult>(GenerateQueries.GetActiveWorlds, parameters)).ToArray();

        var result = new GetActiveWorldsResult() { Worlds = activeWorlds};

        return result;
    }
}
