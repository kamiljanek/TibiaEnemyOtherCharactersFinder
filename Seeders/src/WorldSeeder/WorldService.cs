namespace WorldSeeder;

public class WorldService : IWorldService
{
    private readonly IWorldSeeder _worldSeeder;

    public WorldService(WorldSeederDecorator worldSeeder)
    {
        _worldSeeder = worldSeeder;
    }

    public async Task Run()
    {
        await _worldSeeder.SetProperties();
        await _worldSeeder.Seed();
        await _worldSeeder.TurnOffIfWorldIsUnavailable();
    }
}