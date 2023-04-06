using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationUpdater : ISeeder
{
    private readonly IRepository _repository;

    public CharacterCorrelationUpdater(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Seed()
    {
        await _repository.UpdateCharacterCorrelations();
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlUpdateCharacterCorrelationIfExist);
    }
}