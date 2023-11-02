using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace WorldScanSeeder;

public interface IScanSeeder : ISeeder<World>
{
    List<World> AvailableWorlds { get; }
    Task SetProperties();
}