using Microsoft.Extensions.Logging;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector;

public class ChangeNameDetectorService : IChangeNameDetectorService
{
    private readonly ILogger<ChangeNameDetectorService> _logger;
    private readonly IRepository _repository;
    private readonly ITibiaDataService _tibiaDataService;
    private readonly IEventBusPublisher _busPublisher;

    public ChangeNameDetectorService(ILogger<ChangeNameDetectorService> logger,
        IRepository repository,
        ITibiaDataService tibiaDataService,
        IEventBusPublisher busPublisher)
    {
        _logger = logger;
        _repository = repository;
        _tibiaDataService = tibiaDataService;
        _busPublisher = busPublisher;
    }

    public async Task Run()
    {
        while (true)
        {
            _repository.ClearChangeTracker();
            var character = await _repository.GetFirstCharacterByVerifiedDateAsync();
            if (character is null)
            {
                return;
            }
            var fechedCharacter = await _tibiaDataService.FetchCharacter(character.Name);

            // If Character was not Traded and Character Name is still in database just Update Verified Date.
            if (!IsCharacterChangedName(fechedCharacter, character) && !IsCharacterTraded(fechedCharacter))
            {
                _logger.LogInformation("Character '{characterName}' was not traded", character.Name);
            }


            // If TibiaData cannot find character just delete with all correlations.
            else if (!IsCharacterExist(fechedCharacter))
            {
                await _busPublisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteCharacterWithCorrelationsEvent(character.CharacterId));
            }


            // If Character was Traded just delete all correlations.
            else if (IsCharacterTraded(fechedCharacter))
            {
                await _busPublisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteCharacterCorrelationsEvent(character.CharacterId));
            }


            // If name from databese was found in former names than merge proper correlations.
            else if (IsCharacterFoundInFormerNames(fechedCharacter, character))
            {
                var newCharacter = await _repository.GetCharacterByNameAsync(fechedCharacter.characters.character.name);

                // If new character name is not yet in the databese just proceed.
                if (newCharacter is not null)
                {
                    await _busPublisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                        new MergeTwoCharactersEvent(character.CharacterId, newCharacter.CharacterId));
                }
            }

            character.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
            await _repository.SaveChangesAsync();
        }
    }

    private static bool IsCharacterFoundInFormerNames(TibiaDataCharacterInformationResult fechedCharacter, Character character)
    {
        return fechedCharacter.characters.character.former_names.Any(n => string.Equals(n, character.Name, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsCharacterChangedName(TibiaDataCharacterInformationResult fechedCharacter, Character character)
    {
        return fechedCharacter.characters.character.name?.ToLower() != character.Name;
    }

    private static bool IsCharacterTraded(TibiaDataCharacterInformationResult fechedCharacter)
    {
        return fechedCharacter.characters.character.traded;
    }

    private static bool IsCharacterExist(TibiaDataCharacterInformationResult fechedCharacter)
    {
        return !string.IsNullOrWhiteSpace(fechedCharacter.characters.character.name);
    }
}