namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;

public interface ITibiaApi
{
    public Task<List<string>> FetchWorldsNamesFromTibiaApi();
    public Task<string> FetchCharactersOnlineFromApi(string name);
}