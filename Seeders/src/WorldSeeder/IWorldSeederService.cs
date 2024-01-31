using TibiaEnemyOtherCharactersFinder.Application.Interfaces;

namespace WorldSeeder;

public interface IWorldSeederService : ISeederService
{
    Task SetProperties();
    Task TurnOffIfWorldIsUnavailable();
    Task TurnOnIfWorldIsAvailable();

}