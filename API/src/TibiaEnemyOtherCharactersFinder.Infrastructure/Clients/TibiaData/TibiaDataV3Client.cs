using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

public class TibiaDataV3Client : ITibiaDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TibiaDataV3Client> _logger;
    private readonly string _apiVersion;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TibiaDataV3Client(HttpClient httpClient, IOptions<TibiaDataSection> tibiaData, ILogger<TibiaDataV3Client> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiVersion = tibiaData.Value.ApiVersion;
        _retryPolicy = Policy.Handle<TaskCanceledException>().Or<Exception>().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task<IReadOnlyList<string>> FetchWorldsNames()
    {
        var retryCount = 1;

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}worlds");
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataV3WorldsResponse>(content);
                if (string.IsNullOrWhiteSpace(contentDeserialized.Worlds.RecordDate))
                {
                    throw new TibiaDataApiEmptyResponseException();
                }
                var worldNames = contentDeserialized.Worlds.RegularWorlds.Select(world => world.Name).ToArray();

                return worldNames;
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("{exceptionName} during invoke method {method}, attempt {retryCount}.",
                    nameof(TaskCanceledException), nameof(FetchWorldsNames), retryCount);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError("Method {method} problem, attempt {retryCount}. Exception {exception}",
                    nameof(FetchWorldsNames), retryCount, exception);
                throw;
            }
        });
    }

    public async Task<IReadOnlyList<string>> FetchCharactersOnline(string worldName)
    {
        var retryCount = 1;

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}world/{worldName}");
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogInformation("Server '{serverName}' is off.", worldName);
                    return Array.Empty<string>();
                }

                var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataV3WorldResponse>(content);
                if (string.IsNullOrWhiteSpace(contentDeserialized.Worlds.World.CreationDate))
                {
                    throw new TibiaDataApiEmptyResponseException();
                }

                if (contentDeserialized.Worlds.World.OnlinePlayers is null || !contentDeserialized.Worlds.World.OnlinePlayers.Any())
                {
                    _logger.LogInformation("Server '{serverName}' is out of players at that moment.", worldName);
                    return Array.Empty<string>();
                }

                return contentDeserialized.Worlds.World.OnlinePlayers.Select(x => x.Name).ToArray();
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogError(
                    "{exceptionName} during invoke method {method}, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(TaskCanceledException), nameof(FetchCharactersOnline), worldName, retryCount++, exception);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Method {method} problem, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharactersOnline), worldName, retryCount++, exception);
                throw;
            }
        });
    }

    public async Task<CharacterResult> FetchCharacter(string characterName)
    {
        for (int retryCount = 1; retryCount <= 6; retryCount++)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}character/{characterName}");
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataV3CharacterResponse>(content);
                if (string.IsNullOrWhiteSpace(contentDeserialized.Characters.Character.Name))
                {
                    if (retryCount == 6)
                    {
                        return contentDeserialized.MapToCharacterResult();
                    }

                    throw new TibiaDataApiEmptyResponseException();
                }

                return contentDeserialized.MapToCharacterResult();
            }
            catch (TaskCanceledException)
            {
                _logger.LogError(
                    "{exceptionName} during invoke method {method}, character: '{characterName}', attempt {retryCount}.",
                    nameof(TaskCanceledException), nameof(FetchCharacter), characterName, retryCount);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method {method} problem, character: '{characterName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharacter), characterName, retryCount, exception.Message);
            }
        }

        return null;
    }
}