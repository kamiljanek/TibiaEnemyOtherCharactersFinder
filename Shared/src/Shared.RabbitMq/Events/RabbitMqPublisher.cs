using System.Text;
using RabbitMQ.Client;

namespace Shared.RabbitMQ.Events;

public class RabbitMqPublisher
{
    private const string Hostname = "localhost";
    private const string ExchangeName = "message_exchange";

    public static void PublishMessage(string messageType, string message)
    {
        var factory = new ConnectionFactory() { HostName = Hostname };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Deklaracja exchange
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);

            var publisherProperties = channel.CreateBasicProperties();
            publisherProperties.Persistent = true;

            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(ExchangeName, messageType, publisherProperties, body);

            Console.WriteLine($"Published message: {message}");
        }
    }
}