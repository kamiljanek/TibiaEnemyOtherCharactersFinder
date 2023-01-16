namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public interface IWorldSeeder : ISeeder
{
    public Task SetProperties();
    public Task TurnOffIfWorldIsUnavailable();
}