using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;

namespace RabbitMqSubscriber.Events;

public class DeleteCharacterCorrelationsEventSubscriber : IEventSubscriber
{
    private readonly ILogger<DeleteCharacterCorrelationsEventSubscriber> _logger;
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly IRepository _repository;

    public DeleteCharacterCorrelationsEventSubscriber(
        ILogger<DeleteCharacterCorrelationsEventSubscriber> logger,
        IRabbitMqConventionProvider conventionProvider,
        IRepository repository)
    {
        _logger = logger;
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

        var character = await _repository.GetCharacterByIdAsync(eventObject.CharacterId, cancellationToken);

        await _repository.ExecuteInTransactionAsync(async () =>
        {
            await _repository.DeleteCharacterCorrelationsByCharacterIdAsync(character.CharacterId, cancellationToken);
            character.TradedDate = DateOnly.FromDateTime(DateTime.Now);
        });
    }
}