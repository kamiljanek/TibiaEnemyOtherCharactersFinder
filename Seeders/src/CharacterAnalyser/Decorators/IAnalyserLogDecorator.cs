using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace CharacterAnalyser.Decorators;

public interface IAnalyserLogDecorator : IParameterizedLoggerDecorator<List<WorldScan>>, ILoggerDecorator<List<WorldScan>>
{
}