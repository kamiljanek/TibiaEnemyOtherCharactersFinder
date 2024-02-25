using System.Diagnostics;
using ChangeNameDetector.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace ChangeNameDetector.Services;

public class ChangeNameDetectorService : IChangeNameDetectorService
{
    private readonly ILogger<ChangeNameDetectorService> _logger;
    private readonly INameDetectorValidator _validator;
    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly ITibiaDataClient _tibiaDataClient;
    private readonly IEventPublisher _publisher;

    public ChangeNameDetectorService(ILogger<ChangeNameDetectorService> logger,
        INameDetectorValidator validator,
        ITibiaCharacterFinderDbContext dbContext,
        ITibiaDataClient tibiaDataClient,
        IEventPublisher publisher)
    {
        _logger = logger;
        _validator = validator;
        _dbContext = dbContext;
        _tibiaDataClient = tibiaDataClient;
        _publisher = publisher;
    }

    public async Task Run()
    {
        while (true)
        {
            var stopwatch = Stopwatch.StartNew();

            var character = await GetFirstCharacterByVerifiedDateAsync();
            if (character is null)
            {
                break;
            }

            var fechedCharacter = await _tibiaDataClient.FetchCharacter(character.Name);
            if (fechedCharacter is null)
            {
                continue;
            }

            // If Character was not Traded and Character Name is still in database just Update Verified Date.
            if (!_validator.IsCharacterChangedName(fechedCharacter, character) && !_validator.IsCharacterTraded(fechedCharacter))
            {
                stopwatch.Stop();
                _logger.LogInformation("Character '{characterName}' was not traded, was not changed name. Checked in execution time : {time} ms",
                    character.Name, stopwatch.ElapsedMilliseconds);
            }


            // If TibiaData cannot find character just delete with all correlations.
            else if (!_validator.IsCharacterExist(fechedCharacter))
            {
                await _publisher.PublishAsync($"'{character.Name}' ({DateTime.Now})",
                    new DeleteCharacterWithCorrelationsEvent(character.CharacterId));
            }


            // If Character was Traded just delete all correlations.
            else if (_validator.IsCharacterTraded(fechedCharacter))
            {
                await _publisher.PublishAsync($"'{character.Name}' ({DateTime.Now})",
                    new DeleteCharacterCorrelationsEvent(character.CharacterId));
            }


            // If name from databese was found in former names than merge proper correlations.
            else
            {
                var fechedCharacterName = fechedCharacter.Name;

                var newCharacter = await _dbContext.Characters.Where(c => c.Name == fechedCharacterName.ToLower()).FirstOrDefaultAsync();

                if (newCharacter is null)
                {
                    // If new character name is not yet in the databese just change old name to new one.
                    await UpdateCharacterNameAsync(character.Name, fechedCharacterName);
                    _logger.LogInformation("Character name '{character}' updated to '{newCharacter}'", character.Name, fechedCharacterName.ToLower());
                }
                else
                {
                    await _publisher.PublishAsync($"'{character.Name}' / '{newCharacter.Name}' ({DateTime.Now})",
                        new MergeTwoCharactersEvent(character.CharacterId, newCharacter.CharacterId));
                }
            }

            await UpdateCharacterVerifiedDate(character.CharacterId);
            _dbContext.ChangeTracker.Clear();
        }
    }

    private async Task<Character> GetFirstCharacterByVerifiedDateAsync()
    {
        var visibilityOfTradeProperty = DateOnly.FromDateTime(DateTime.Now.AddDays(-31));
        var scanPeriod = DateOnly.FromDateTime(DateTime.Now.AddDays(-15));

        return await _dbContext.Characters
            .Where(c => (!c.TradedDate.HasValue || c.TradedDate < visibilityOfTradeProperty)
                        && (!c.VerifiedDate.HasValue || c.VerifiedDate < scanPeriod))
            .OrderByDescending(c => c.VerifiedDate == null)
            .ThenBy(c => c.VerifiedDate)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    private async Task UpdateCharacterNameAsync(string oldName, string newName)
    {
        await _dbContext.Characters
            .Where(c => c.Name == oldName.ToLower())
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.Name, newName.ToLower()));
    }

    private async Task UpdateCharacterVerifiedDate(int characterId)
    {
        await _dbContext.Characters
            .Where(c => c.CharacterId == characterId)
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.VerifiedDate, DateOnly.FromDateTime(DateTime.Now)));
    }
}