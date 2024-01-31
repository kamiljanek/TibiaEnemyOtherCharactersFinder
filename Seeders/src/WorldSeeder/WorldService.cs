using WorldSeeder.Decorators;

namespace WorldSeeder;

public class WorldService : IWorldService
{
    private readonly IWorldSeederService _worldSeederService;
    private readonly IWorldSeederLogDecorator _logDecorator;

    public WorldService(IWorldSeederService worldSeederService, IWorldSeederLogDecorator logDecorator)
    {
        _worldSeederService = worldSeederService;
        _logDecorator = logDecorator;
    }

    public async Task Run()
    {
        await _logDecorator.Decorate(_worldSeederService.SetProperties);
        await _logDecorator.Decorate(_worldSeederService.Seed);
        await _logDecorator.Decorate(_worldSeederService.TurnOffIfWorldIsUnavailable);
        await _logDecorator.Decorate(_worldSeederService.TurnOnIfWorldIsAvailable); // TODO: add unit test for that
    }
}