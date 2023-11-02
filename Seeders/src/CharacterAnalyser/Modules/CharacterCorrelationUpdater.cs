using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationUpdater : ISeederService
{
    private readonly IRepository _repository;

    public CharacterCorrelationUpdater(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Seed()
    {
        await _repository.UpdateCharacterCorrelationsAsync();
    }
}