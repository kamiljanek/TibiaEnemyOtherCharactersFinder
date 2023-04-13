using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaDataApi;

public interface ITibiaDataApiClient
{
    public Task<List<string>> FetchWorldsNamesFromTibiaApi();
    public Task<string> FetchCharactersOnlineFromTibiaApi(string name);
    Task<TibiaApiCharacterInformationResult> FetchCharacterFromTibiaApi(string name);
}