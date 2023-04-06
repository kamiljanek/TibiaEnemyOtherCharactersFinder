using Newtonsoft.Json;
using System.IO.Compression;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Providers.DataProvider;

public class TibiaDataProvider : ITibiaDataProvider
{
    private const string _baseUrl = $"https://api.tibiadata.com/v3/";
    private const string _worldsEndpoint = $"worlds/";
    private const string _worldEndpoint = $"world/";
    private readonly HttpClient _httpClient;

    public TibiaDataProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> FetchWorldsNamesFromTibiaApi()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{_worldsEndpoint}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await ReadContentAsString(response);
            var contentDeserialized = JsonConvert.DeserializeObject<TibiaApiWorldsResult>(content);
            var worldNames = contentDeserialized.worlds.regular_worlds.Select(world => world.name).ToList();

            Console.WriteLine($"Status Code during 'Fetching Worlds names from Tibia Api' is success");
            return worldNames;
        }

        Console.WriteLine($"Status Code during 'Fetching Worlds names from Tibia Api' isn't success");
        return new List<string>();
    }

    public async Task<string> FetchCharactersOnlineFromTibiaApi(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{_worldEndpoint}{name}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await ReadContentAsString(response);

            var contentDeserialized = JsonConvert.DeserializeObject<TibiaApiWorldInformationResult>(content);

            var onlinePlayers = contentDeserialized.worlds.world.online_players.Select(x => x.name).ToList();

            Console.WriteLine($"{name} - successfull");

            return string.Join("|", onlinePlayers);
        }
        Console.WriteLine($"{name} - code isn't success");

        return string.Empty;
    }

    private async Task<string> ReadContentAsString(HttpResponseMessage response)
    {
        // Check whether response is compressed
        if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
        {
            // Decompress manually
            using (var s = await response.Content.ReadAsStreamAsync())
            {
                using (var decompressed = new GZipStream(s, CompressionMode.Decompress))
                {
                    using (var rdr = new StreamReader(decompressed))
                    {
                        return await rdr.ReadToEndAsync();
                    }
                }
            }
        }
        // Use standard implementation if not compressed
        return await response.Content.ReadAsStringAsync();
    }
}