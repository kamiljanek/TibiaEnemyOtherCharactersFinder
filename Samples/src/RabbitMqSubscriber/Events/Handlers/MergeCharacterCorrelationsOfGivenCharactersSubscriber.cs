using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Events;

namespace RabbitMqSubscriber.Events.Handlers;

public class MergeCharacterCorrelationsOfGivenCharactersSubscriber : IEventSubscriber
{
    private readonly ILogger<MergeCharacterCorrelationsOfGivenCharactersSubscriber> _logger;

    public MergeCharacterCorrelationsOfGivenCharactersSubscriber(ILogger<MergeCharacterCorrelationsOfGivenCharactersSubscriber> logger)
    {
        _logger = logger;
    }

    public string GetQueueName()
    {
        return "tibia-eocf.merge_correlations_of_given_characters_event";
    }


    public Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
    {
        var payload = Encoding.UTF8.GetString(ea.Body.Span);
        var eventObject = JsonConvert.DeserializeObject<MergeTwoCharactersEvent>(payload);
        Console.WriteLine($"odebrane {eventObject.NewCharacterId}/{eventObject.OldCharacterId}");
        _logger.LogInformation("Event subscribed. Payload: {Payload}", payload);
        return Task.CompletedTask;
    }
}