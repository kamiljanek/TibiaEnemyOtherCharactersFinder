using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaDataApi;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class TibiaDataApiService : ITibiaDataService
{
    private readonly ITibiaDataApiClient _tibiaDataApiClient;

    public TibiaDataApiService(ITibiaDataApiClient tibiaDataApiClient)
    {
        _tibiaDataApiClient = tibiaDataApiClient;
    }

    public async Task<List<string>> FetchWorldsNames()
    {
        return await _tibiaDataApiClient.FetchWorldsNamesFromTibiaApi();
    }

    public async Task<string> FetchCharactersOnline(string name)
    {
        return await _tibiaDataApiClient.FetchCharactersOnlineFromTibiaApi(name);
    }

    public async Task<TibiaApiCharacterInformationResult> FetchCharacter(string name)
    {
        return await _tibiaDataApiClient.FetchCharacterFromTibiaApi(name);
    }
}