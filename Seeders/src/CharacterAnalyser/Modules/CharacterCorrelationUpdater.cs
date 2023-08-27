using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

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
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlUpdateCharacterCorrelationIfExist);
    }
}