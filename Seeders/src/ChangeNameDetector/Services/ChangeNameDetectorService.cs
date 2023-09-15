using ChangeNameDetector.Validators;
using Microsoft.Extensions.Logging;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;

namespace ChangeNameDetector.Services;

public class ChangeNameDetectorService : IChangeNameDetectorService
{
    private readonly ILogger<ChangeNameDetectorService> _logger;
    private readonly INameDetectorValidator _validator;
    private readonly IRepository _repository;
    private readonly ITibiaDataService _tibiaDataService;
    private readonly IEventPublisher _publisher;

    public ChangeNameDetectorService(ILogger<ChangeNameDetectorService> logger,
        INameDetectorValidator validator,
        IRepository repository,
        ITibiaDataService tibiaDataService,
        IEventPublisher publisher)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _tibiaDataService = tibiaDataService;
        _publisher = publisher;
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
            if (!_validator.IsCharacterChangedName(fechedCharacter, character) && !_validator.IsCharacterTraded(fechedCharacter))
            {
                _logger.LogInformation("Character '{characterName}' was not traded", character.Name);
            }


            // If TibiaData cannot find character just delete with all correlations.
            else if (!_validator.IsCharacterExist(fechedCharacter))
            {
                await _publisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteCharacterWithCorrelationsEvent(character.CharacterId));
            }


            // If Character was Traded just delete all correlations.
            else if (_validator.IsCharacterTraded(fechedCharacter))
            {
                await _publisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteCharacterCorrelationsEvent(character.CharacterId));
            }


            // If name from databese was found in former names than merge proper correlations.
            else if (_validator.IsCharacterFoundInFormerNames(fechedCharacter, character))
            {
                var newCharacter = await _repository.GetCharacterByNameAsync(fechedCharacter.characters.character.name);

                // If new character name is not yet in the databese just proceed.
                if (newCharacter is not null)
                {
                    await _publisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                        new MergeTwoCharactersEvent(character.CharacterId, newCharacter.CharacterId));
                }
            }

            character.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
            await _repository.SaveChangesAsync();
        }
    }
}