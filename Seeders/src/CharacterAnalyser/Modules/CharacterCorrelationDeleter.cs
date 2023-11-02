using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationDeleter
{
    private readonly IRepository _repository;

    public CharacterCorrelationDeleter(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Delete()
    {
        await _repository.DeleteCharacterCorrelationIfCorrelationExistInScanAsync();
    }
}