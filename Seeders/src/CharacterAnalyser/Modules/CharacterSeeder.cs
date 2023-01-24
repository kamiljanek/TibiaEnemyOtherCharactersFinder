using Dapper;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterSeeder : ISeeder
{
    private readonly IDapperConnectionProvider _connectionProvider;

    public CharacterSeeder(IDapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task Seed()
    {
        using var connection = _connectionProvider.GetConnection(EDataBaseType.PostgreSql);
            await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);
    }
}