using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Handlers;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace RabbitMqSubscriber.Events;

public class DeleteCharacterCorrelationsEventSubscriber : IEventSubscriber
{
    private readonly ILogger<DeleteCharacterCorrelationsEventSubscriber> _logger;
    private readonly IEventResultHandler _eventResultHandler;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly IRepository _repository;

    public DeleteCharacterCorrelationsEventSubscriber(
        ILogger<DeleteCharacterCorrelationsEventSubscriber> logger,
        IEventResultHandler eventResultHandler,
        IRabbitMqConventionProvider conventionProvider,
        IRepository repository)
    {
        _logger = logger;
        _eventResultHandler = eventResultHandler;
        _conventionProvider = conventionProvider;
        _repository = repository;
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


        var isCommitedProperly = await _repository.ExecuteInTransactionAsync(async () =>
        {
            var character = await _repository.GetCharacterByIdAsync(eventObject.CharacterId, cancellationToken: cancellationToken);
            await _repository.DeleteCharacterCorrelationsByCharacterIdAsync(character.CharacterId);
            await _repository.UpdateCharacterTradedDate(character.CharacterId);
        });

        _eventResultHandler.HandleTransactionResult(isCommitedProperly, nameof(DeleteCharacterCorrelationsEvent), payload);
    }
}