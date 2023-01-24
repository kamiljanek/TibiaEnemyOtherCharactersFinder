using Dapper;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;

namespace DbCleaner;

public class Cleaner : ICleaner
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public Cleaner(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task ClearDeletedWorldScans()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        await connection.ExecuteAsync(GenerateQueries.NpgsqlClearDeletedWorldScans);
    }
}