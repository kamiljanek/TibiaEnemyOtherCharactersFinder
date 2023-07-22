using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Application.Services;

public interface ITibiaDataService
{
    public Task<List<string>> FetchWorldsNames();
    public Task<string> FetchCharactersOnline(string name);
    Task<TibiaDataCharacterInformationResult> FetchCharacter(string name);
}