using DbCleaner.Decorators;

namespace DbCleaner;

public class CleanerService : ICleanerService
{
    private readonly ICleaner _cleaner;
    private readonly IDbCleanerLogDecorator _logDecorator;

    public CleanerService(ICleaner cleaner, IDbCleanerLogDecorator logDecorator)
    {
        _cleaner = cleaner;
        _logDecorator = logDecorator;
    }

    public async Task Run()
    {
        await _logDecorator.Decorate(_cleaner.ClearUnnecessaryWorldScans);
        await _logDecorator.Decorate(_cleaner.TruncateCharacterActions);
        await _logDecorator.Decorate(_cleaner.DeleteIrrelevantCharacterCorrelations);
        await _logDecorator.Decorate(_cleaner.VacuumCharacterActions);
        await _logDecorator.Decorate(_cleaner.VacuumWorldScans);
        await _logDecorator.Decorate(_cleaner.VacuumCharacters);
        await _logDecorator.Decorate(_cleaner.VacuumWorlds);
        await _logDecorator.Decorate(_cleaner.VacuumCharacterCorrelations);
    }
}