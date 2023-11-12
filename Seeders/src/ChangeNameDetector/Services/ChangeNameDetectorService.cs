using ChangeNameDetector.Validators;
using Microsoft.Extensions.Logging;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace ChangeNameDetector.Services;

public class ChangeNameDetectorService : IChangeNameDetectorService
{
    private readonly ILogger<ChangeNameDetectorService> _logger;
    private readonly INameDetectorValidator _validator;
    private readonly IRepository _repository;
    private readonly ITibiaDataClient _tibiaDataClient;
    private readonly IEventPublisher _publisher;

    public ChangeNameDetectorService(ILogger<ChangeNameDetectorService> logger,
        INameDetectorValidator validator,
        IRepository repository,
        ITibiaDataClient tibiaDataClient,
        IEventPublisher publisher)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _tibiaDataClient = tibiaDataClient;
        _publisher = publisher;
    }

    public async Task Run()
    {
        while (true)
        {
            var character = await _repository.GetFirstCharacterByVerifiedDateAsync();
            if (character is null)
            {
                return;
            }

            var fechedCharacter = await _tibiaDataClient.FetchCharacter(character.Name);
            if (fechedCharacter is null)
            {
                continue;
            }

            // If Character was not Traded and Character Name is still in database just Update Verified Date.
            if (!_validator.IsCharacterChangedName(fechedCharacter, character) && !_validator.IsCharacterTraded(fechedCharacter))
            {
                _logger.LogInformation("Character '{characterName}' was not traded, was not changed name", character.Name);
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
                var newCharacter = await _repository.GetCharacterByNameAsync(fechedCharacter.characters.character.name);

                if (newCharacter is null)
                {
                    // If new character name is not yet in the databese just change old name to new one.
                    await _repository.UpdateCharacterNameAsync(character.Name, fechedCharacter.characters.character.name);
                    _logger.LogInformation("Character name '{character}' updated to '{newCharacter}'", character.Name, fechedCharacter.characters.character.name.ToLower());
                }
                else
                {
                    await _publisher.PublishAsync($"'{character.Name}' / '{newCharacter.Name}' ({DateTime.Now})",
                        new MergeTwoCharactersEvent(character.CharacterId, newCharacter.CharacterId));
                }
            }

            await _repository.UpdateCharacterVerifiedDate(character.CharacterId);
            await _repository.ClearChangeTracker();
        }
    }
}