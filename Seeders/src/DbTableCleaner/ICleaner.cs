namespace DbCleaner;

public interface ICleaner
{
    Task ClearTables();
    Task VacuumTables();
}