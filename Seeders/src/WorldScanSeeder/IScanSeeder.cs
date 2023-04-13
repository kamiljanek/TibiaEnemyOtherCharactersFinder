using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public interface IScanSeeder : ISeeder<World>
{
    List<World> AvailableWorlds { get; }
    Task SetProperties();
}