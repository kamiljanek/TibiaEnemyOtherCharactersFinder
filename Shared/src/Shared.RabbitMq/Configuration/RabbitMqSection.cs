using System.ComponentModel.DataAnnotations;

namespace Shared.RabbitMQ.Configuration;

#pragma warning disable CS8618

public class RabbitMqSection
{
    public const string SectionName = "RabbitMq";


    [Required] public int Retries { get; init; }
    [Required] public int RetryInterval { get; init; }
    [Required] public string Username { get; init; }
    [Required] public string Password { get; init; }
    [Required] public string VirtualHost { get; init; }
    [Required] public int Port { get; init; }
    [Required] public string HostUrl { get; init; }
    public ExchangeOptions Exchange { get; init; }
    public QueueOptions Queue { get; init; }
    public DeadLetterOptions DeadLetter { get; init; }
}

public class ExchangeOptions
{
    [Required] public string Name { get; init; }
    [Required] public bool Durable { get; init; }
    [Required] public bool AutoDelete { get; init; }
}

public class QueueOptions
{
    [Required] public bool Durable { get; init; }
    [Required] public bool AutoDelete { get; init; }
    [Required] public bool Exclusive { get; init; }
}

public class DeadLetterOptions
{
    [Required] public bool Durable { get; init; }
    [Required] public bool AutoDelete { get; init; }
    [Required] public bool Exclusive { get; init; }
    [Required] public string Prefix { get; init; }
}
