using Dapper;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace DbTableCleaner;

public class Cleaner : ICleaner
{
    private readonly IDapperConnectionProvider _connectionProvider;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public Cleaner(IDapperConnectionProvider connectionProvider, ITibiaCharacterFinderDbContext dbContext)
    {
        _connectionProvider = connectionProvider;
        _dbContext = dbContext;
    }

    public async Task ClearDeletedWorldScans()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
        await connection.ExecuteAsync(GenerateQueries.NpgsqlClearDeletedWorldScans);
    }
}
