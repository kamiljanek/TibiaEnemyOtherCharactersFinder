namespace DbCleaner;

public class CleanerService : ICleanerService
{
    private readonly CleanerDecorator _cleaner;

    public CleanerService(CleanerDecorator cleaner)
    {
        _cleaner = cleaner;
    }

    public async Task Run()
    {
        await _cleaner.ClearTables();
        await _cleaner.VacuumTables();
    }
}