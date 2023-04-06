namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;

public interface ITibiaDataProvider
{
    public Task<List<string>> FetchWorldsNamesFromTibiaApi();
    public Task<string> FetchCharactersOnlineFromTibiaApi(string name);
}