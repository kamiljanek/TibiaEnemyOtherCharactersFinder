using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Dtos;
using RabbitMqSubscriber.Subscribers;
using Shared.Database.Queries.Sql;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace RabbitMqSubscriber.Events;

public class MergeTwoCharactersEventSubscriber : IEventSubscriber
{
    private readonly ILogger<MergeTwoCharactersEventSubscriber> _logger;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly IRepository _repository;

    public MergeTwoCharactersEventSubscriber(ILogger<MergeTwoCharactersEventSubscriber> logger, IRabbitMqConventionProvider conventionProvider, IRepository repository)
    {
        _logger = logger;
        _conventionProvider = conventionProvider;
        _repository = repository;
    }

    public string GetQueueName()
    {
        var queueBinding = _conventionProvider.GetForType<MergeTwoCharactersEvent>();
        return queueBinding.Queue;
    }

    public async Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
    {
        var payload = Encoding.UTF8.GetString(ea.Body.Span);
        var eventObject = JsonConvert.DeserializeObject<MergeTwoCharactersEvent>(payload);
        _logger.LogInformation("Event {Event} subscribed. Payload: {Payload}", eventObject.GetType().Name, payload);

        var oldCharacter = await _repository.GetCharacterByIdAsync(eventObject.OldCharacterId, cancellationToken);
        var newCharacter = await _repository.GetCharacterByIdAsync(eventObject.NewCharacterId, cancellationToken);

        await _repository.ReplaceCharacterIdInCorrelationsAsync(oldCharacter, newCharacter, cancellationToken);
        List<CharacterCorrelation> correlations = new();
        List<string> combinedCharacterCorrelations = new();
        // List<CombinedCharacterCorrelation> combinedCharacterCorrelations = new();
        List<int> correlationIdsToDelete = new();

        var sameCharacterCorrelations =
            (await _repository.SqlQueryRaw<string>(GenerateQueries.NpgsqlGetSameCharacterCorrelations,
                newCharacter.CharacterId)).ToList();

        var sameCharacterCorrelationsCrossed =
            (await _repository.SqlQueryRaw<string>(GenerateQueries.NpgsqlGetSameCharacterCorrelationsCrossed,
                newCharacter.CharacterId)).ToList();

        combinedCharacterCorrelations.AddRange(sameCharacterCorrelations);
        combinedCharacterCorrelations.AddRange(sameCharacterCorrelationsCrossed);

        foreach (var row in combinedCharacterCorrelations)
        {
            var combinedCorrelation = JsonConvert.DeserializeObject<CombinedCharacterCorrelation>(row);
            correlationIdsToDelete.Add(combinedCorrelation.FirstCombinedCorrelation.CorrelationId);
            correlationIdsToDelete.Add(combinedCorrelation.SecondCombinedCorrelation.CorrelationId);
            // combinedCharacterCorrelations.Add(combinedCorrelation);
            var characterCorrelation = PrepareCharacterCorrelation(combinedCorrelation);
            correlations.Add(characterCorrelation);
        }

        // var correlationIdsToDelete = combinedCharacterCorrelations
        //     .SelectMany(c => new[] { c.FirstCombinedCorrelation.CorrelationId, c.SecondCombinedCorrelation.CorrelationId })
        //     .ToArray();
        // UNDONE: sprawdzić czy tego już nie potrzebuję wraz z zakomentowanymi liniami wyżej


        // UNDONE: poniższe operacje na bazie zamknąć w jedną transakcje

        await _repository.ExecuteInTransactionAsync(async () =>
        {
            await _repository.AddRangeAsync(correlations, cancellationToken);
            await _repository.DeleteAsync(oldCharacter, cancellationToken);

            // Delete already merged CharacterCorrelations
            await _repository.DeleteCharacterCorrelationsByIdsAsync(correlationIdsToDelete, cancellationToken);

            oldCharacter.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
            // await _repository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    private CharacterCorrelation PrepareCharacterCorrelation(CombinedCharacterCorrelation combinedCorrelation)
    {
        return new CharacterCorrelation()
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
        };
    }
}