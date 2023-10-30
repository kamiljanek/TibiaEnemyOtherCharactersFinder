using TibiaEnemyOtherCharactersFinder.Application.Services;

namespace WorldSeeder;

public interface IWorldSeederService : ISeederService
{
    public Task SetProperties();
    public Task TurnOffIfWorldIsUnavailable();
}