using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace RabbitMqSubscriber.Events;

public class DeleteCharacterWithCorrelationsEventSubscriber : IEventSubscriber
{
    private readonly ILogger<DeleteCharacterWithCorrelationsEventSubscriber> _logger;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly IRepository _repository;

    public DeleteCharacterWithCorrelationsEventSubscriber(
        ILogger<DeleteCharacterWithCorrelationsEventSubscriber> logger,
        IRabbitMqConventionProvider conventionProvider,
        IRepository repository)
    {
        _logger = logger;
        _conventionProvider = conventionProvider;
        _repository = repository;
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

        var character = await _repository.GetCharacterByIdAsync(eventObject.CharacterId, cancellationToken);
        await _repository.DeleteAsync(character, cancellationToken);
    }
}