using System.Data;

namespace Shared.Providers

{
    public interface IDapperConnectionProvider
    {
        IDbConnection GetConnection(EModuleType eModule);
        string GetConnectionString(EModuleType eModule);
    }
}