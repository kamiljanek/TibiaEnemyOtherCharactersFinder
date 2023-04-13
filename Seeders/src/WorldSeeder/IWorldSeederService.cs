using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public interface IWorldSeederService : ISeederService
{
    public Task SetProperties();
    public Task TurnOffIfWorldIsUnavailable();
}