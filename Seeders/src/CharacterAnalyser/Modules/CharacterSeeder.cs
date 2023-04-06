using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterSeeder : ISeeder
{
    private readonly IRepository _repository;

    public CharacterSeeder(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Seed()
    {

        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);

    }
}