using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterAnalyserCleaner
{
    private readonly IRepository _repository;

    public CharacterAnalyserCleaner(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearCharacterActionsAsync()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlClearCharacterActions);
    }
}