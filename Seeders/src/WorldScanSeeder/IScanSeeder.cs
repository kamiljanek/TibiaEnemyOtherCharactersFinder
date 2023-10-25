using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace WorldScanSeeder;

public interface IScanSeeder : ISeeder<World>
{
    List<World> AvailableWorlds { get; }
    Task SetProperties();
}