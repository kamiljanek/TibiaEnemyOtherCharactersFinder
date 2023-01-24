using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public interface IWorldScanSeeder : ISeeder<World>
{
    List<World> AvailableWorlds { get; }
    Task SetProperties();
}