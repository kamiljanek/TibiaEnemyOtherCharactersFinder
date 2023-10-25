using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;

namespace CharacterAnalyser.Modules;

public class CharacterSeederService : ISeederService
{
    private readonly IRepository _repository;

    public CharacterSeederService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Seed()
    {

        await _repository.ExecuteRawSqlAsync(GenerateQueries.CreateCharacterIfNotExist);

    }
}