namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public interface ICharacterAnalyser : ISeeder
{
    public Task SetProperties();
}