using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Application.Interfaces;

public interface ITibiaDataClient
{
    public Task<IReadOnlyList<string>> FetchWorldsNames();
    public Task<IReadOnlyList<string>> FetchCharactersOnline(string worldName);
    Task<CharacterResult> FetchCharacter(string characterName, bool withRetryPolicy = true);
}