using System.Data;

namespace TibiaEnemyOtherCharactersFinderApi.Providers

{
    public interface IDapperConnectionProvider
    {
        IDbConnection GetConnection(EModuleType eModule);
    }
}