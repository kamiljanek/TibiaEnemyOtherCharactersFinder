using System.IO.Compression;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaDataApi;

public class TibiaDataApiClient : ITibiaDataApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiVersion;

    public TibiaDataApiClient(HttpClient httpClient, IOptions<TibiaDataApiSection> tibiaDataApi)
    {
        _httpClient = httpClient;
        _apiVersion = tibiaDataApi.Value.ApiVersion;
    }

    public async Task<List<string>> FetchWorldsNamesFromTibiaApi()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/worlds");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await ReadContentAsString(response);
            var contentDeserialized = JsonConvert.DeserializeObject<TibiaApiWorldsResult>(content);
            var worldNames = contentDeserialized.worlds.regular_worlds.Select(world => world.name).ToList();

            return worldNames;
        }

        return new List<string>();
    }

    public async Task<string> FetchCharactersOnlineFromTibiaApi(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/world/{name}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await ReadContentAsString(response);

            var contentDeserialized = JsonConvert.DeserializeObject<TibiaApiWorldInformationResult>(content);

            var onlinePlayers = contentDeserialized.worlds.world.online_players.Select(x => x.name).ToList();

            return string.Join("|", onlinePlayers);
        }

        return string.Empty;
    }

    public async Task<TibiaApiCharacterInformationResult> FetchCharacterFromTibiaApi(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/character/{name}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await ReadContentAsString(response);

            return JsonConvert.DeserializeObject<TibiaApiCharacterInformationResult>(content);
        }

        return null;
    }

    private async Task<string> ReadContentAsString(HttpResponseMessage response)
    {
        // Check whether response is compressed
        if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
        {
            // Decompress manually
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var decompressed = new GZipStream(stream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(decompressed);
            return await streamReader.ReadToEndAsync();
        }
        // Use standard implementation if not compressed
        return await response.Content.ReadAsStringAsync();
    }
}