using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class TibiaDataService : ITibiaDataService
{
    private readonly ITibiaDataClient _tibiaDataClient;

    public TibiaDataService(ITibiaDataClient tibiaDataClient)
    {
        _tibiaDataClient = tibiaDataClient;
    }

    public async Task<List<string>> FetchWorldsNames()
    {
        return await _tibiaDataClient.FetchWorldsNames();
    }

    public async Task<string> FetchCharactersOnline(string name)
    {
        return await _tibiaDataClient.FetchCharactersOnline(name);
    }

    public async Task<TibiaDataCharacterInformationResult> FetchCharacter(string name)
    {
        return await _tibiaDataClient.FetchCharacter(name);
    }
}