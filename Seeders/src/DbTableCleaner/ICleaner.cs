namespace DbCleaner;

public interface ICleaner
{
    Task ClearDeletedWorldScans();
    Task TruncateCharacterActions();
    Task DeleteIrrelevantCharacterCorrelations();
    Task VacuumCharacterActions();
    Task VacuumWorldScans();
    Task VacuumCharacters();
    Task VacuumWorlds();
    Task VacuumCharacterCorrelations();
}