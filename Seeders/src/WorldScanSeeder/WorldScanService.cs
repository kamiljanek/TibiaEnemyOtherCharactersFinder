using WorldScanSeeder.Decorators;

namespace WorldScanSeeder;

public class WorldScanService : IWorldScanService
{
    private readonly IScanSeeder _scanSeeder;
    private readonly IScanSeederLogDecorator _logDecorator;

    public WorldScanService(IScanSeeder scanSeeder, IScanSeederLogDecorator logDecorator)
    {
        _scanSeeder = scanSeeder;
        _logDecorator = logDecorator;
    }

    public async Task Run()
    {
        await _scanSeeder.SetProperties();

        foreach (var availableWorld in _scanSeeder.AvailableWorlds)
        {
            await _logDecorator.Decorate(_scanSeeder.Seed, availableWorld);
        }
    }
}