using Microsoft.Extensions.Logging;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

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
            await _repository.ClearChangeTracker();
            var character = await _repository.GetFirstCharacterByVerifiedDate();
            var fechedCharacter = await _tibiaDataService.FetchCharacter(character.Name);


            // If Character was not Traded and Character Name is still in database just Update Verified Date.
            if (fechedCharacter.characters.character.name?.ToLower() == character.Name &&
                !fechedCharacter.characters.character.traded)
            {
            }


            // If TibiaData cannot find character just delete with all correlations.
            else if (IsCharacterExist(fechedCharacter))
            {
                await _busPublisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteCharacterWithCorrelationsEvent(character.CharacterId));

                // await _repository.DeleteAsync(character);
            }


            // If Character was Traded just delete all correlations.
            else if (IsCharacterTraded(fechedCharacter))
            {
                await _busPublisher.PublishAsync($"{character.Name}-{DateTime.Now}",
                    new DeleteChcaracterCorrelationsEvent(character.CharacterId));
                // UNDONE: zamknąć poniższe metody w jednej transakcji
                // await _repository.DeleteCharacterCorrelationsByCharacterId(character.CharacterId);
                // character.TradedDate = DateOnly.FromDateTime(DateTime.Now);
            }


            // If name from databese was found in former names than merge proper correlations.
            else
            {
                var newCharacter = await _repository.GetCharacterByName(fechedCharacter.characters.character.name);

                // If new character name is not yet in the databese just proceed.
                if (newCharacter is not null)
                {
                    await _busPublisher.PublishAsync($"{character.Name}/{newCharacter.Name}-{DateTime.Now}",
                        new MergeTwoCharactersEvent(character.CharacterId, newCharacter.CharacterId));
                }

                // await _correlationService.Merge(character, newCharacter);
            }

            character.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
            await _repository.SaveChangesAsync();
        }
    }

    private static bool IsCharacterTraded(TibiaDataCharacterInformationResult fechedCharacter)
    {
        return fechedCharacter.characters.character.traded;
    }

    private static bool IsCharacterExist(TibiaDataCharacterInformationResult fechedCharacter)
    {
        if (string.IsNullOrWhiteSpace(fechedCharacter.characters.character.name))
        {
            return false;
        }

        return true;
    }
}