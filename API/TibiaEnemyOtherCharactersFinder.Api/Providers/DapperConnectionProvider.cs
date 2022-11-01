using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace TibiaEnemyOtherCharactersFinder.Api.Providers

{
    public class DapperConnectionProvider : IDapperConnectionProvider
    {
        private readonly DapperConfigurationSection _dapperOptions;
        private readonly ConnectionStringsSection _connectionStringsOptions;

        public DapperConnectionProvider(IOptions<DapperConfigurationSection> options, IOptions<ConnectionStringsSection> connectionStringsOptions)
        {
            _connectionStringsOptions = connectionStringsOptions.Value;
            _dapperOptions = options.Value;
        }

        public IDbConnection GetConnection(EModuleType module)
        {
            switch (module)
            {
                case EModuleType.SqlServer:
                    return GetConnection(_connectionStringsOptions.SqlServer, _dapperOptions.CommandTimeout);

                case EModuleType.PostgreSql:
                    return GetConnection(_connectionStringsOptions.PostgreSql, _dapperOptions.CommandTimeout);

                default:
                    throw new ArgumentOutOfRangeException(nameof(module), module, null);
            }
        }
        public string GetConnectionString(EModuleType module)
        {
            switch (module)
            {
                case EModuleType.SqlServer:
                    return _connectionStringsOptions.SqlServer;

                case EModuleType.PostgreSql:
                    return _connectionStringsOptions.PostgreSql;

                default:
                    throw new ArgumentOutOfRangeException(nameof(module), module, null);
            }
        }

        private static IDbConnection GetConnection(string connectionString, int? commandTimeout)
        {
            var connection = new SqlConnection(connectionString);
            SqlMapper.Settings.CommandTimeout = commandTimeout;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            connection.Open();

            return connection;
        }
    }
}