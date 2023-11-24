using System.Transactions;
using Microsoft.Extensions.Logging;

namespace RabbitMqSubscriber.Handlers;

public class EventResultHandler : IEventResultHandler
{
    private readonly ILogger<EventResultHandler> _logger;

    public EventResultHandler(ILogger<EventResultHandler> logger)
    {
        _logger = logger;
    }
    public void HandleTransactionResult(bool isCommitedProperly, string eventName, string payload)
    {
        switch (isCommitedProperly)
        {
            case true:
                _logger.LogInformation("Transaction '{event}' commited properly. Payload {payload}",
                    eventName, payload);
                break;
            case false:
                _logger.LogError("Transaction '{event}' failed. Check dead letter for analyse problem. Payload {payload}",
                    eventName, payload);
                throw new TransactionException($"Transaction commited inproperly during event '{eventName}'. Payload: {payload}.");
        }
    }
}