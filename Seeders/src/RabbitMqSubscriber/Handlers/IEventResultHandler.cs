namespace RabbitMqSubscriber.Handlers;

public interface IEventResultHandler
{
    void HandleTransactionResult(bool isCommitedProperly, string eventName, string payload, string characterName);
}