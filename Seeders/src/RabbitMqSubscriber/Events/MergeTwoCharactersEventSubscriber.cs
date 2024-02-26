using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Dtos;
using RabbitMqSubscriber.Handlers;
using RabbitMqSubscriber.Subscribers;
using Shared.Database.Queries.Sql;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace RabbitMqSubscriber.Events;

public class MergeTwoCharactersEventSubscriber : IEventSubscriber
{
    private readonly ILogger<MergeTwoCharactersEventSubscriber> _logger;
    private readonly IEventResultHandler _eventResultHandler;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public MergeTwoCharactersEventSubscriber(
        ILogger<MergeTwoCharactersEventSubscriber> logger,
        IEventResultHandler eventResultHandler,
        IRabbitMqConventionProvider conventionProvider,
        ITibiaCharacterFinderDbContext dbContext)
    {
        _logger = logger;
        _eventResultHandler = eventResultHandler;
        _conventionProvider = conventionProvider;
        _dbContext = dbContext;
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

        var oldCharacter = await _dbContext.Characters
            .Where(c => c.CharacterId == eventObject.OldCharacterId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var newCharacter = await _dbContext.Characters
            .Where(c => c.CharacterId == eventObject.NewCharacterId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (oldCharacter is null || newCharacter is null)
        {
            _logger.LogInformation(
                "During event {Event} - cannot find character one of the characters in database. Payload: {Payload}",
                eventObject.GetType().Name, payload);
            return;
        }

        var isCommitedProperly = await ExecuteInTransactionAsync(async () =>
        {
            await ReplaceCharacterIdInCorrelationsAsync(oldCharacter, newCharacter);
            List<CharacterCorrelation> correlations = new();
            List<string> combinedCharacterCorrelations = new();
            List<long> correlationIdsToDelete = new();

            var sameCharacterCorrelations = _dbContext.Database
                .SqlQueryRaw<string>(GenerateQueries.GetSameCharacterCorrelations, newCharacter.CharacterId).AsEnumerable();

            var sameCharacterCorrelationsCrossed = _dbContext.Database
                .SqlQueryRaw<string>(GenerateQueries.GetSameCharacterCorrelationsCrossed, newCharacter.CharacterId).AsEnumerable();

            combinedCharacterCorrelations.AddRange(sameCharacterCorrelations);
            combinedCharacterCorrelations.AddRange(sameCharacterCorrelationsCrossed);

            foreach (var row in combinedCharacterCorrelations)
            {
                var combinedCorrelation = JsonConvert.DeserializeObject<CombinedCharacterCorrelation>(row);
                correlationIdsToDelete.Add(combinedCorrelation.FirstCombinedCorrelation.CorrelationId);
                correlationIdsToDelete.Add(combinedCorrelation.SecondCombinedCorrelation.CorrelationId);
                var characterCorrelation = PrepareCharacterCorrelation(combinedCorrelation);
                correlations.Add(characterCorrelation);
            }


            _dbContext.CharacterCorrelations.AddRange(correlations);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Delete already merged CharacterCorrelations
            await _dbContext.CharacterCorrelations
                .Where(c => correlationIdsToDelete.Contains(c.CorrelationId))
                .ExecuteDeleteAsync(cancellationToken);
        });

        _eventResultHandler.HandleTransactionResult(isCommitedProperly, nameof(MergeTwoCharactersEvent), payload);

        await _dbContext.Characters
            .Where(c => c.CharacterId == oldCharacter.CharacterId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private async Task<bool> ExecuteInTransactionAsync(Func<Task> action)
    {
        for (int retryCount = 1; retryCount <= 3; retryCount++)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                await action.Invoke();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Method {method} during {action} failed, attempt {retryCount}. Error message: {ErrorMessage}",
                    nameof(ExecuteInTransactionAsync), action.Target?.GetType().ReflectedType?.Name, retryCount, ex.Message);
            }
        }

        return false;
    }

    private async Task ReplaceCharacterIdInCorrelationsAsync(Character oldCharacter, Character newCharacter)
    {
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LoginCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LoginCharacterId, newCharacter.CharacterId));
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LogoutCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LogoutCharacterId, newCharacter.CharacterId));
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