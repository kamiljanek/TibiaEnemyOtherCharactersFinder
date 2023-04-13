namespace WorldSeeder;

public class WorldService : IWorldService
{
    private readonly IWorldSeederService _worldSeederService;

    public WorldService(WorldSeederServiceDecorator worldSeederService)
    {
        _worldSeederService = worldSeederService;
    }

    public async Task Run()
    {
        await _worldSeederService.SetProperties();
        await _worldSeederService.Seed();
        await _worldSeederService.TurnOffIfWorldIsUnavailable();
    }
}