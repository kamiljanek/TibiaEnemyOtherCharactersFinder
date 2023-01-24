using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public interface ICharacterAnalyser : ISeeder<List<WorldScan>>
{
    List<short> UniqueWorldIds { get; }

    Task<bool> HasDataToAnalyse();

    Task<List<WorldScan>> GetWorldScansToAnalyseAsync(short worldId);
}