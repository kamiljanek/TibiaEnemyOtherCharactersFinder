using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

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