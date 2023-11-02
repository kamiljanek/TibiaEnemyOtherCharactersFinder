using TibiaEnemyOtherCharactersFinder.Application.Interfaces;

namespace WorldSeeder;

public interface IWorldSeederService : ISeederService
{
    public Task SetProperties();
    public Task TurnOffIfWorldIsUnavailable();
}