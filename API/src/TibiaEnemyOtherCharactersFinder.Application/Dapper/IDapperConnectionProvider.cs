using System.Data;

namespace TibiaEnemyOtherCharactersFinder.Application.Dapper;

public interface IDapperConnectionProvider
{
    IDbConnection GetConnection(EDataBaseType eModule);
    //string GetConnectionString(EDataBaseType eModule);
    // UNDONE: mozliwe ze do usuniecia
}