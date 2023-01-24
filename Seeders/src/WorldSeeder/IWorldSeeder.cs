using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public interface IWorldSeeder : ISeeder
{
    public Task SetProperties();
    public Task TurnOffIfWorldIsUnavailable();
}