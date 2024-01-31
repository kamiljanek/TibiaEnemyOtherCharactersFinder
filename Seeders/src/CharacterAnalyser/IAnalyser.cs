using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace CharacterAnalyser;

public interface IAnalyser : ISeeder<List<WorldScan>>
{
    Task<List<short>> GetDistinctWorldIdsFromRemainingScans();
    Task<List<WorldScan>> GetWorldScansToAnalyseAsync(short worldId);
}