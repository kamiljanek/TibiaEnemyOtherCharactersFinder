using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

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
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlDeleteCharacterCorrelationIfCorrelationExistInScan);
        await _repository.DeleteCharacterCorrelationIfCorrelationExistInScan();
        await _repository.ClearChangeTracker();
    }
}