using Dapper;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationDeleter
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public CharacterCorrelationDeleter(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task Delete()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
             await connection.ExecuteAsync(GenerateQueries.NpgsqlDeleteCharacterCorrelationIfCorrelationExistInOneScan);
    }
}