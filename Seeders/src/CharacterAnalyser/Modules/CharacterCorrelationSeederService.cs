using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationSeederService : ISeederService
{
    private readonly IRepository _repository;

    public CharacterCorrelationSeederService(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Seed()
    {
        await _repository.CreateCharacterCorrelationsIfNotExistAsync();
    }
}