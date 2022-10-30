using System.Data;

namespace TibiaEnemyOtherCharactersFinder.Api.Providers

{
    public interface IDapperConnectionProvider
    {
        IDbConnection GetConnection(EModuleType eModule);
    }
}