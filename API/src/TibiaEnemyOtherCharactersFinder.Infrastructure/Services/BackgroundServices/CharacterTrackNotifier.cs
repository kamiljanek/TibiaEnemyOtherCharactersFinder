using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Application.SignalR;
using TibiaEnemyOtherCharactersFinder.Application.SignalR.Events;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services.BackgroundServices;

public class CharacterTrackNotifier : BackgroundService
{
    private readonly TimeSpan _period;

    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly ICharactersTrackHubWrapper _trackHubWrapper;

    public CharacterTrackNotifier(ITibiaCharacterFinderDbContext dbContext, ICharactersTrackHubWrapper trackHubWrapper, IOptions<BackgroundServiceTimerSection> options)
    {
        _dbContext = dbContext;
        _trackHubWrapper = trackHubWrapper;
        _period = TimeSpan.FromSeconds(options.Value.CharacterTrackNotifier);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (await _dbContext.Database.CanConnectAsync(stoppingToken))
            {
                var uniqueNames = _dbContext.TrackedCharacters.Select(tc => tc.Name).Distinct();
                if (uniqueNames.Any())
                {
                    var foundedNames = _dbContext.OnlineCharacters.Where(oc => uniqueNames.Contains(oc.Name))
                        .Select(oc => oc.Name);

                    var notFoundedNames = uniqueNames.Except(foundedNames);

                    var listOfData = new List<CharacterTrackedHubEvent>();
                    listOfData.AddRange(foundedNames.Select(foundedName => new CharacterTrackedHubEvent(foundedName, true)));
                    listOfData.AddRange(notFoundedNames.Select(notFoundedName => new CharacterTrackedHubEvent(notFoundedName, false)));

                    foreach (var data in listOfData)
                    {
                        await _trackHubWrapper.PublishToGroupAsync(data.Name, data);
                    }
                }
            }
        }
    }
}