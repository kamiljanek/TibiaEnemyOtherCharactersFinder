using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterCorrelationSeeder : ISeeder
{
    private readonly IRepository _repository;

    public CharacterCorrelationSeeder(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Seed()
    {
        await _repository.CreateCharacterCorrelationsIfNotExist();
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlCreateCharacterCorrelationIfNotExist);
    }
}