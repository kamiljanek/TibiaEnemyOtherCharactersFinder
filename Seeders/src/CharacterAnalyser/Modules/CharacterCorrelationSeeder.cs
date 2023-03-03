using Shared.Database.Queries.Sql;
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
        await _repository.UpdateCharacterCorrelations();
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlUpdateCharacterCorrelationIfExist);
        Console.WriteLine($" - {DateTime.Now.ToLongTimeString()} - updated CharactersCorrelations");

        await _repository.CreateCharacterCorrelationsIfNotExist();
        // await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlCreateCharacterCorrelationIfNotExist);
        Console.WriteLine($" - {DateTime.Now.ToLongTimeString()} - created new CharactersCorrelations");
    }
}