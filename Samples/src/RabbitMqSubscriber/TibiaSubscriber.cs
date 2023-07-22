using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqSubscriber.Events.Handlers;
using Shared.RabbitMQ.Configuration;

namespace RabbitMqSubscriber;

public class TibiaSubscriber
{
    private readonly IEnumerable<IEventSubscriber> _subscribers;
    private readonly ILogger<TibiaSubscriber> _logger;
    private readonly SubscriberConnection _connection;

    public TibiaSubscriber(IEnumerable<IEventSubscriber> subscribers, ILogger<TibiaSubscriber> logger, SubscriberConnection connection)
    {
        _subscribers = subscribers;
        _logger = logger;
        _connection = connection;
    }

    public void Subscribe()
    {
        foreach (var eventSubscriber in _subscribers)
        {
            RegisterConsumer(_connection.Connection, eventSubscriber);
        }
    }

    private void RegisterConsumer(IConnection connection, IEventSubscriber eventConsumer)
    {
        IModel channel = connection.CreateModel();
        channel.BasicQos(0, 1, true);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, deliveryArguments) =>
        {
            var retryCount = 1;

            if (retryCount >= 2)
            {
                channel.BasicReject(deliveryArguments.DeliveryTag, false);
                return;
            }

            try
            {
                await eventConsumer.OnReceived(deliveryArguments);
                channel.BasicAck(deliveryTag: deliveryArguments.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on received message: {Message}", ex.Message);
                channel.BasicReject(deliveryArguments.DeliveryTag, false);
            }
        };

        try
        {

            channel.BasicConsume(queue: eventConsumer.GetQueueName(), autoAck: false, consumer: consumer);

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error on register consumer {Consumer}: {Message}",
                eventConsumer.GetType(),
                ex.Message);
        }
    }

    // private static int GetRetryCount(IBasicProperties messageProperties)
    // {
    //      var result = messageProperties.Headers is not null && messageProperties.Headers.ContainsKey(RetryHeaderName)
    //         ? (int)messageProperties.Headers[RetryHeaderName] : 0;
    //
    //      return result;
    // }
}