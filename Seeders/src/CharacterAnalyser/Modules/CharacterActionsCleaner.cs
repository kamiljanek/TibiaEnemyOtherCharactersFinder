using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterActionsCleaner
{
    private readonly IRepository _repository;

    public CharacterActionsCleaner(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearAsync()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlClearCharacterActions);
    }

    public async Task ResetFoundInScanOnCharactersAsync()
    {
        // await _repository.ResetCharacterFoundInScan();
        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlUpdateCharactersSetFoundInScanFalse);
    }
}