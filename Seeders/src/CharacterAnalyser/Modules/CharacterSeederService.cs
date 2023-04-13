using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

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

        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);

    }
}