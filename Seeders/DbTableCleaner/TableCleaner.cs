using Dapper;
using Shared.Database.Queries.Sql;
using Shared.Providers;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace DbTableCleaner
{
    public class TableCleaner
    {
        private readonly IDapperConnectionProvider _connectionProvider;

        public TableCleaner(IDapperConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task CleanWorldScansTable()
        {
            using var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql);
            await connection.ExecuteAsync(GenerateQueries.NpgsqlClearWorldScans);
        }
    }
}
