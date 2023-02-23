using Dapper;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationSeeder : ISeeder
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public CharacterCorrelationSeeder(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task Seed()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
            await connection.ExecuteAsync(GenerateQueries.NpgsqlUpdateCharacterCorrelationIfExist);
            await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterCorrelationIfNotExist);
    }
}