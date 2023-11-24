using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

public class TibiaDataClient : ITibiaDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TibiaDataClient> _logger;
    private readonly string _apiVersion;


    public TibiaDataClient(HttpClient httpClient, IOptions<TibiaDataSection> tibiaData, ILogger<TibiaDataClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiVersion = tibiaData.Value.ApiVersion;
    }

    public async Task<List<string>> FetchWorldsNames()
    {
        for (int retryCount = 0; retryCount < 3; retryCount++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/worlds");

            try
            {
                using var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldsResult>(content);
                    var worldNames = contentDeserialized.worlds.regular_worlds.Select(world => world.name).ToList();

                    return worldNames;
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("TaskCanceledException during invoke method {method}, attempt {retryCount}.",
                    nameof(FetchWorldsNames), retryCount + 1);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method {method} problem. Exception {exception}",
                    nameof(FetchWorldsNames), exception);
            }
        }

        return new List<string>();
    }

    public async Task<string> FetchCharactersOnline(string worldName)
    {
        for (int retryCount = 0; retryCount < 3; retryCount++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/world/{worldName}");

            try
            {
                using var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogInformation("Server '{serverName}' is off.", worldName);
                        return string.Empty;
                    }

                    var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldInformationResult>(content);
                    if (contentDeserialized.worlds.world.online_players is null || !contentDeserialized.worlds.world.online_players.Any())
                    {
                        _logger.LogInformation("Server '{serverName}' is out of players at that moment.", worldName);
                        return string.Empty;
                    }

                    var onlinePlayers = contentDeserialized.worlds.world.online_players.Select(x => x.name).ToList();
                    return string.Join("|", onlinePlayers);
                }
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError("TaskCanceledException during invoke method {method}, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharactersOnline), worldName, retryCount + 1, exception);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method {method} problem, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharactersOnline), worldName, retryCount + 1, exception);
            }
        }

        return string.Empty;
    }

    public async Task<TibiaDataCharacterInformationResult> FetchCharacter(string characterName)
    {
        for (int retryCount = 0; retryCount < 3; retryCount++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/character/{characterName}");

            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TibiaDataCharacterInformationResult>(content);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("TaskCanceledException during invoke method {method}, character: '{characterName}', attempt {retryCount}.",
                    nameof(FetchCharacter), characterName, retryCount + 1);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method {method} problem. Exception {exception}",
                    nameof(FetchCharacter), exception);
            }
        }

        return null;
    }
}