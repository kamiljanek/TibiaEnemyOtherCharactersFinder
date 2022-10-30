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
                case EModuleType.TibiaDB:
                    return GetConnection(_connectionStringsOptions.TibiaDB, _dapperOptions.CommandTimeout);

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