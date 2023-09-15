using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Configuration;

namespace RabbitMqSubscriber.Subscribers;

public class TibiaSubscriber
{
    private readonly IEnumerable<IEventSubscriber> _subscribers;
    private readonly ILogger<TibiaSubscriber> _logger;
    private readonly RabbitMqConnection _connection;
    private readonly RabbitMqSection _options;
    private readonly SceduleOptions _scedule;
    private const string RetryHeaderName = "x-redelivered-count";

    public TibiaSubscriber(IEnumerable<IEventSubscriber> subscribers, ILogger<TibiaSubscriber> logger, RabbitMqConnection connection, IOptions<RabbitMqSection> options)
    {
        _subscribers = subscribers;
        _logger = logger;
        _connection = connection;
        _scedule = options.Value.Scedule;
        _options = options.Value;
    }

    public void Subscribe()
    {
        foreach (var eventSubscriber in _subscribers)
        {
            RegisterConsumer(eventSubscriber);
        }
    }

    // public void Stop()
    // {
        // _shouldRun = false;
    // }
    // UNDONE: wywalic powyższy kod jeżeli subscriber bedzie dobrze działał (musze przetestować ręczenie czy zbiera wiadomości i sam sie nie wyłącza)

    // TODO: teraz włączyć debugowanie 2 projektów na raz i sprawdzić czy subscriber działa w odpowiednich godzinach i czy się nie wyłącza
    private void RegisterConsumer(IEventSubscriber eventSubscriber)
    {
        IModel channel = _connection.Connection.CreateModel();
        channel.BasicQos(0, 1, true);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, deliveryArguments) =>
        {
            var retryCount = GetRetryCount(deliveryArguments.BasicProperties);

            if (retryCount >= _options.Retries)
            {
                channel.BasicReject(deliveryArguments.DeliveryTag, false);
                return;
            }

            try
            {
                await eventSubscriber.OnReceived(deliveryArguments);
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
            // UNDONE: sprawdzić czy rabbit pobiera wiadomości w odpowiednich godzinach
            if (IsSubscriptionTime())
            {
                channel.BasicConsume(queue: eventSubscriber.GetQueueName(), autoAck: false, consumer: consumer);
            }
            else
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                // UNDONE: zmienić czas na jakiś lepsz
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error on register consumer {Consumer}: {Message}",
                eventSubscriber.GetType(),
                ex.Message);
        }
    }

    private bool IsSubscriptionTime()
    {
        var currentTime = DateTime.Now.TimeOfDay;
        var startSubscribeTime = TimeSpan.Parse(_scedule.StartSubscribeTime);
        var endSubscribeTime = TimeSpan.Parse(_scedule.EndSubscribeTime);

        return _scedule.Enabled && currentTime >= startSubscribeTime && currentTime <= endSubscribeTime;
    }

    private static int GetRetryCount(IBasicProperties messageProperties)
    {
         var result = messageProperties.Headers is not null && messageProperties.Headers.ContainsKey(RetryHeaderName)
            ? (int)messageProperties.Headers[RetryHeaderName] : 0;

         return result;
    }
    // UNDONE: poprawić zwykłe appsettings.json
}