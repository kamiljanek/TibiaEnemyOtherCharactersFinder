using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DapperProvider;

public class DapperConnectionProvider : IDapperConnectionProvider
{
    private readonly DapperConfigurationSection _dapperOptions;
    private readonly ConnectionStringsSection _connectionStringsOptions;

    public DapperConnectionProvider(IOptions<DapperConfigurationSection> options, IOptions<ConnectionStringsSection> connectionStringsOptions)
    {
        _connectionStringsOptions = connectionStringsOptions.Value;
        _dapperOptions = options.Value;
    }

    public IDbConnection GetConnection(EDataBaseType module)
    {
        switch (module)
        {
            case EDataBaseType.SqlServer:
                return GetConnection(_connectionStringsOptions.SqlServer, _dapperOptions.CommandTimeout);

            case EDataBaseType.PostgreSql:
                return GetConnection(_connectionStringsOptions.PostgreSql, _dapperOptions.CommandTimeout);

            default:
                throw new ArgumentOutOfRangeException(nameof(module), module, null);
        }
    }

    private static IDbConnection GetConnection(string connectionString, int? commandTimeout)
    {
        var connection = new NpgsqlConnection(connectionString);
        SqlMapper.Settings.CommandTimeout = commandTimeout;
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        connection.Open();

        return connection;
    }
}