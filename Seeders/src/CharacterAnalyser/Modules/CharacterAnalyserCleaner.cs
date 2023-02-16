using Dapper;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;

namespace CharacterAnalyser.Modules;

public class CharacterAnalyserCleaner
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public CharacterAnalyserCleaner(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task ClearCharacterActionsAsync()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        await connection.ExecuteAsync(GenerateQueries.NpgsqlClearCharacterActions);
    }
}