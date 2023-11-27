using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace WorldScanSeeder.Decorators;

public interface IScanSeederLogDecorator : IParameterizedLoggerDecorator<World>
{
}