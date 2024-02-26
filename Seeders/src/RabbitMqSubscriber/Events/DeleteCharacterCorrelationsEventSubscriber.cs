using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Handlers;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace RabbitMqSubscriber.Events;

public class DeleteCharacterCorrelationsEventSubscriber : IEventSubscriber
{
    private readonly ILogger<DeleteCharacterCorrelationsEventSubscriber> _logger;
    private readonly IEventResultHandler _eventResultHandler;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public DeleteCharacterCorrelationsEventSubscriber(
        ILogger<DeleteCharacterCorrelationsEventSubscriber> logger,
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
        var queueBinding = _conventionProvider.GetForType<DeleteCharacterCorrelationsEvent>();
        return queueBinding.Queue;
    }

    public async Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
    {
        var payload = Encoding.UTF8.GetString(ea.Body.Span);
        var eventObject = JsonConvert.DeserializeObject<DeleteCharacterCorrelationsEvent>(payload);
        _logger.LogInformation("Event {Event} subscribed. Payload: {Payload}", eventObject.GetType().Name, payload);


        var isCommitedProperly = await ExecuteInTransactionAsync(async () =>
        {
            var character = await _dbContext.Characters
                .Where(c => c.CharacterId == eventObject.CharacterId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            await _dbContext.CharacterCorrelations
                .Where(c => c.LoginCharacterId == character.CharacterId || c.LogoutCharacterId == character.CharacterId)
                .ExecuteDeleteAsync(cancellationToken);

            await _dbContext.Characters
                .Where(c => c.CharacterId == character.CharacterId)
                .ExecuteUpdateAsync(update => update
                    .SetProperty(c => c.TradedDate, DateOnly.FromDateTime(DateTime.Now)), cancellationToken);
        });

        _eventResultHandler.HandleTransactionResult(isCommitedProperly, nameof(DeleteCharacterCorrelationsEvent), payload);
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