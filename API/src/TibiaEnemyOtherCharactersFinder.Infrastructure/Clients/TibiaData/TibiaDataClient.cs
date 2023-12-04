using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

public class TibiaDataClient : ITibiaDataClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TibiaDataClient> _logger;
    private readonly string _apiVersion;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TibiaDataClient(HttpClient httpClient, IOptions<TibiaDataSection> tibiaData, ILogger<TibiaDataClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiVersion = tibiaData.Value.ApiVersion;
        _retryPolicy = Policy.Handle<TaskCanceledException>().Or<Exception>().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task<IReadOnlyList<string>> FetchWorldsNames()
    {
        var currentRetry = 0;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/worlds");

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldsResult>(content);
                var worldNames = contentDeserialized.worlds.regular_worlds.Select(world => world.name).ToArray();

                return worldNames;
            }
            catch (TaskCanceledException)
            {
                currentRetry++;
                _logger.LogError("TaskCanceledException during invoke method {method}, attempt {retryCount}.",
                    nameof(FetchWorldsNames), currentRetry);
            }
            catch (Exception exception)
            {
                currentRetry++;
                _logger.LogError("Method {method} problem, attempt {retryCount}. Exception {exception}",
                    nameof(FetchWorldsNames), currentRetry, exception);
            }

            return Array.Empty<string>();
        });
    }

    public async Task<IReadOnlyList<string>> FetchCharactersOnline(string worldName)
    {
        var currentRetry = 0;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/world/{worldName}");

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogInformation("Server '{serverName}' is off.", worldName);
                    return Array.Empty<string>();
                }

                var contentDeserialized = JsonConvert.DeserializeObject<TibiaDataWorldInformationResult>(content);
                if (contentDeserialized.worlds.world.online_players is null || !contentDeserialized.worlds.world.online_players.Any())
                {
                    _logger.LogInformation("Server '{serverName}' is out of players at that moment.", worldName);
                    return Array.Empty<string>();
                }

                return contentDeserialized.worlds.world.online_players.Select(x => x.name).ToArray();
            }
            catch (TaskCanceledException exception)
            {
                currentRetry++;
                _logger.LogError("TaskCanceledException during invoke method {method}, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharactersOnline), worldName, currentRetry, exception);
            }
            catch (Exception exception)
            {
                currentRetry++;
                _logger.LogError("Method {method} problem, world: '{worldName}', attempt {retryCount}. Exception {exception}",
                    nameof(FetchCharactersOnline), worldName, currentRetry, exception);
            }

            return Array.Empty<string>();
        });
    }

    public async Task<TibiaDataCharacterInformationResult> FetchCharacter(string characterName)
    {
        var currentRetry = 0;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiVersion}/character/{characterName}");
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TibiaDataCharacterInformationResult>(content);
            }
            catch (TaskCanceledException)
            {
                currentRetry++;
                _logger.LogError("TaskCanceledException during invoke method {method}, character: '{characterName}', attempt {retryCount}.",
                    nameof(FetchCharacter), characterName, currentRetry);
            }
            catch (Exception exception)
            {
                currentRetry++;
                _logger.LogError("Method {method} problem, attempt {retryCount}. Exception {exception}", nameof(FetchCharacter), currentRetry, exception);
            }

            return null;
        });
    }
}