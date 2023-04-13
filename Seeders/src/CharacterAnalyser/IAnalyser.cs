using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public interface IAnalyser : ISeeder<List<WorldScan>>
{
    List<short> UniqueWorldIds { get; }

    Task<bool> HasDataToAnalyse();

    Task<List<WorldScan>> GetWorldScansToAnalyseAsync(short worldId);
}