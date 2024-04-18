using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Handlers;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace RabbitMqSubscriber.Events;

public class DeleteCharacterWithCorrelationsEventSubscriber : IEventSubscriber
{
    private readonly ILogger<DeleteCharacterWithCorrelationsEventSubscriber> _logger;
    private readonly IEventResultHandler _eventResultHandler;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public DeleteCharacterWithCorrelationsEventSubscriber(
        ILogger<DeleteCharacterWithCorrelationsEventSubscriber> logger,
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
        var queueBinding = _conventionProvider.GetForType<DeleteCharacterWithCorrelationsEvent>();
        return queueBinding.Queue;
    }

    public async Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
    {
        var payload = Encoding.UTF8.GetString(ea.Body.Span);
        var eventObject = JsonConvert.DeserializeObject<DeleteCharacterWithCorrelationsEvent>(payload);
        _logger.LogInformation("Event {Event} subscribed. Payload: {Payload}", eventObject.GetType().Name, payload);

        Thread.Sleep(3000);

        var character = new Character();
        var isCommitedProperly = await ExecuteInTransactionAsync(async () =>
        {
            character = await _dbContext.Characters
                .Where(c => c.CharacterId == eventObject.CharacterId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            await _dbContext.Characters
                .Where(c => c.CharacterId == eventObject.CharacterId)
                .ExecuteUpdateAsync(update => update
                    .SetProperty(c => c.DeleteApproachNumber, character.DeleteApproachNumber + 1));

            if (character.DeleteApproachNumber > 3)
            {
                await _dbContext.Characters
                    .Where(c => c.CharacterId == character.CharacterId)
                    .ExecuteDeleteAsync(cancellationToken);
            }
        });

        _eventResultHandler.HandleTransactionResult(isCommitedProperly, nameof(DeleteCharacterWithCorrelationsEvent), payload, character.Name);
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
}