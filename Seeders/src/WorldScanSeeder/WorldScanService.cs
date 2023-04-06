namespace WorldScanSeeder;

public class WorldScanService : IWorldScanService
{
    private readonly IScanSeeder _scanSeeder;

    public WorldScanService(WorldScanSeederDecorator scanSeeder)
    {
        _scanSeeder = scanSeeder;
    }

    public async Task Run()
    {
        await _scanSeeder.SetProperties();
        foreach (var availableWorld in _scanSeeder.AvailableWorlds)
        {
            await _scanSeeder.Seed(availableWorld);
        }
    }
}