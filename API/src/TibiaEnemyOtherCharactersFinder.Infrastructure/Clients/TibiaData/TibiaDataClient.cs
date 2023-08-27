using System.IO.Compression;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

public class TibiaDataClient : ITibiaDataClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiVersion;

    public TibiaDataClient(HttpClient httpClient, IOptions<TibiaDataSection> tibiaData)
    {
        _httpClient = httpClient;
        _apiVersion = tibiaData.Value.ApiVersion;
    }

    public async Task<List<string>> FetchWorldsNames()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/worlds");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldsResult>(content);
            var worldNames = contentDeserialized.worlds.regular_worlds.Select(world => world.name).ToList();

            return worldNames;
        }

        return new List<string>();
    }

    public async Task<string> FetchCharactersOnline(string worldName)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/world/{worldName}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldInformationResult>(content);
            var onlinePlayers = contentDeserialized.worlds.world.online_players.Select(x => x.name).ToList();

            return string.Join("|", onlinePlayers);
        }

        return string.Empty;
    }

    public async Task<TibiaDataCharacterInformationResult> FetchCharacter(string characterName)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/character/{characterName}");

        using var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TibiaDataCharacterInformationResult>(content);
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
        // UNDONE: przetestować i możliwe że to wywalenia
    }
}