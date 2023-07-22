using Newtonsoft.Json;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace ChangeNameDetector;

public class CorrelationService
{
    private readonly IRepository _repository;

    public CorrelationService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Merge(Character oldCharacter, Character newCharacter)
    {
        await _repository.ReplaceCharacterIdInCharacterCorrelations(oldCharacter, newCharacter);
        List<CharacterCorrelation> correlations = new();
        List<string> combinedRaws = new();
        List<CombinedCharacterCorrelation> combinedCharacterCorrelations = new();

        var sameCharacterCorrelations =
            (await _repository.SqlQueryRaw<string>(GenerateQueries.NpgsqlGetSameCharacterCorrelations,
                newCharacter.CharacterId)).ToList();

        var sameCharacterCorrelationsCrossed =
            (await _repository.SqlQueryRaw<string>(GenerateQueries.NpgsqlGetSameCharacterCorrelationsCrossed,
                newCharacter.CharacterId)).ToList();

        combinedRaws.AddRange(sameCharacterCorrelations);
        combinedRaws.AddRange(sameCharacterCorrelationsCrossed);

        foreach (var entity in combinedRaws)
        {
            var combinedCorrelation = JsonConvert.DeserializeObject<CombinedCharacterCorrelation>(entity);
            combinedCharacterCorrelations.Add(combinedCorrelation);
            correlations.Add(new CharacterCorrelation()
            {
                LoginCharacterId = combinedCorrelation.FirstCombinedCorrelation.LoginCharacterId,
                LogoutCharacterId = combinedCorrelation.FirstCombinedCorrelation.LogoutCharacterId,
                NumberOfMatches =
                    (short)(combinedCorrelation.FirstCombinedCorrelation.NumberOfMatches +
                            combinedCorrelation.SecondCombinedCorrelation.NumberOfMatches),
                CreateDate =
                    combinedCorrelation.FirstCombinedCorrelation.CreateDate <
                    combinedCorrelation.SecondCombinedCorrelation.CreateDate
                        ? combinedCorrelation.FirstCombinedCorrelation.CreateDate
                        : combinedCorrelation.SecondCombinedCorrelation.CreateDate,
                LastMatchDate =
                    combinedCorrelation.FirstCombinedCorrelation.LastMatchDate >
                    combinedCorrelation.SecondCombinedCorrelation.LastMatchDate
                        ? combinedCorrelation.FirstCombinedCorrelation.LastMatchDate
                        : combinedCorrelation.SecondCombinedCorrelation.LastMatchDate
            });
        }

        var correlationIds = combinedCharacterCorrelations
            .SelectMany(c => new[]
                { c.FirstCombinedCorrelation.CorrelationId, c.SecondCombinedCorrelation.CorrelationId })
            .ToArray();


        // UNDONE: poniższe operacje na bazie zamknąć w jedną transakcje
        await _repository.AddRangeAsync(correlations);
        await _repository.DeleteAsync(oldCharacter);

        // Delete already merged CharacterCorrelations
        await _repository.DeleteCharacterCorrelationsByIds(correlationIds);

        oldCharacter.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
        await _repository.SaveChangesAsync();
    }
}