using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqSubscriber.Subscribers;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMq.Conventions;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.RabbitMqSubscriber;

[Collection("Seeder test collection")]
public class RabbitMqSubscriberTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly MessageSerializer _serializer = new();

    public RabbitMqSubscriberTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task Subscribe_WhenCharacterNotExist_ShouldDeleteCharacterWithCorrelations()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TibiaSubscriber>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqSection>>();

        var publisherConnection = GetRabbitMqConnection(options, "publisher");
        var subscriberConnection = GetRabbitMqConnection(options, "subscriber");

        await _factory.ClearDatabaseAsync(dbContext);
        await SeedDatabaseAsync(dbContext);

        var subscribers = scope.ServiceProvider.GetServices<IEventSubscriber>();
        var message = new DeleteCharacterWithCorrelationsEvent(121);
        var tibiaSubscriber = new TibiaSubscriber(subscribers, logger, subscriberConnection, options);

        PublishRabbitMessagesToQueue(options, publisherConnection, message);

        // Act
        tibiaSubscriber.Subscribe();
        Thread.Sleep(100);
        tibiaSubscriber.Stop();

        publisherConnection.Connection.Close();
        subscriberConnection.Connection.Close();


        // Assert
        var dbContextAfterSubscribe = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterSubscriber = dbContextAfterSubscribe.Characters.AsNoTracking().ToList();
        var characterCorrelationsAfterSubscriber = dbContextAfterSubscribe.CharacterCorrelations.AsNoTracking().ToList();

        charactersAfterSubscriber.Count.Should().Be(3);
        characterCorrelationsAfterSubscriber.Count.Should().Be(3);
    }

    [Fact]
    public async Task Subscribe_WhenCharacterWasTraded_ShouldDeleteCorrelations()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TibiaSubscriber>>();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqSection>>();

        var publisherConnection = GetRabbitMqConnection(options, "publisher");
        var subscriberConnection = GetRabbitMqConnection(options, "subscriber");

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        await _factory.ClearDatabaseAsync(dbContext);
        await SeedDatabaseAsync(dbContext);

        var subscribers = scope.ServiceProvider.GetServices<IEventSubscriber>();
        var message = new DeleteCharacterCorrelationsEvent(121);
        var tibiaSubscriber = new TibiaSubscriber(subscribers, logger, subscriberConnection, options);

        PublishRabbitMessagesToQueue(options, publisherConnection, message);

        // Act
        tibiaSubscriber.Subscribe();
        Thread.Sleep(100);
        tibiaSubscriber.Stop();

        publisherConnection.Connection.Close();
        subscriberConnection.Connection.Close();


        // Assert
        var dbContextAfterSubscribe = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterSubscriber = dbContextAfterSubscribe.Characters.AsNoTracking().ToList();
        var characterCorrelationsAfterSubscriber =
            dbContextAfterSubscribe.CharacterCorrelations.AsNoTracking().ToList();

        charactersAfterSubscriber.Count.Should().Be(4);
        characterCorrelationsAfterSubscriber.Count.Should().Be(3);
    }

    [Fact]
    public async Task Subscribe_WhenCharacterWasFoundInFormerNames_ShouldMergeProperCorrelations()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TibiaSubscriber>>();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqSection>>();

        var publisherConnection = GetRabbitMqConnection(options, "publisher");
        var subscriberConnection = GetRabbitMqConnection(options, "subscriber");

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        await _factory.ClearDatabaseAsync(dbContext);
        await SeedDatabaseAsync(dbContext);

        var subscribers = scope.ServiceProvider.GetServices<IEventSubscriber>();
        var message = new MergeTwoCharactersEvent(120, 121);
        var tibiaSubscriber = new TibiaSubscriber(subscribers, logger, subscriberConnection, options);

        PublishRabbitMessagesToQueue(options, publisherConnection, message);

        // Act
        tibiaSubscriber.Subscribe();
        Thread.Sleep(100);
        tibiaSubscriber.Stop();

        publisherConnection.Connection.Close();
        subscriberConnection.Connection.Close();

        // Assert
        var dbContextAfterSubscribe = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterSubscriber = dbContextAfterSubscribe.Characters.AsNoTracking().ToList();
        var characterCorrelationsAfterSubscriber = dbContextAfterSubscribe.CharacterCorrelations.AsNoTracking().ToList();

        charactersAfterSubscriber.Count.Should().Be(3);
        charactersAfterSubscriber[0].CharacterId.Should().Be(121);
        charactersAfterSubscriber[1].CharacterId.Should().Be(122);
        charactersAfterSubscriber[2].CharacterId.Should().Be(123);
        characterCorrelationsAfterSubscriber.Count.Should().Be(3);
        characterCorrelationsAfterSubscriber.First(cc => cc is { LoginCharacterId: 123, LogoutCharacterId: 121 }).NumberOfMatches.Should().Be(6);
        characterCorrelationsAfterSubscriber.First(cc => cc is { LoginCharacterId: 122, LogoutCharacterId: 121 }).NumberOfMatches.Should().Be(9);
        characterCorrelationsAfterSubscriber.First(cc => cc is { LoginCharacterId: 123, LogoutCharacterId: 122 }).NumberOfMatches.Should().Be(5);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private void PublishRabbitMessagesToQueue(IOptions<RabbitMqSection> optionsSection, RabbitMqConnection connection, object message)
    {
        var options = optionsSection.Value;

        IModel channel = connection.Connection.CreateModel();
        var exchangeOptions = options.Exchange;
        var deadLetterOptions = options.DeadLetter;
        var queueOptions = options.Queue;

        var routingKey = message.GetType().Name.ToRabbitSnakeCase();
        var queueName = $"{exchangeOptions.Name}.{routingKey}";

        Dictionary<string, object> queueArguments = new()
        {
            { "x-dead-letter-exchange", $"{deadLetterOptions.Prefix}{exchangeOptions.Name}" },
            { "x-dead-letter-routing-key", $"{deadLetterOptions.Prefix}{queueName}" }
        };

        channel.QueueDeclare(queue: queueName, queueOptions.Durable, queueOptions.Exclusive, queueOptions.AutoDelete, queueArguments);

        var serializedMessage = _serializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(serializedMessage);

        channel.BasicPublish(exchange: exchangeOptions.Name, routingKey: routingKey, basicProperties: null, body: body);
    }

    private RabbitMqConnection GetRabbitMqConnection(IOptions<RabbitMqSection> optionsSection, string conectionName)
    {
        var options = optionsSection.Value;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(options!.HostUrl),
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            DispatchConsumersAsync = true
        };

        IConnection connection = factory.CreateConnection(conectionName);

        return new RabbitMqConnection(connection);
    }

    private async Task SeedDatabaseAsync(TibiaCharacterFinderDbContext dbContext)
    {
        await dbContext.Worlds.AddRangeAsync(GetWorlds());
        await dbContext.Characters.AddRangeAsync(GetCharacters());
        await dbContext.CharacterCorrelations.AddRangeAsync(GetCharacterCorrelations());

        await dbContext.SaveChangesAsync();
    }

    private IEnumerable<World> GetWorlds()
    {
        return new List<World>
        {
            new() { WorldId = 11, Name = "Damora", IsAvailable = true, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Damora" },
        };
    }

    private IEnumerable<Character> GetCharacters()
    {
        return new List<Character>
        {
            new() {CharacterId = 120, WorldId = 11, Name = "aphov"},
            new() {CharacterId = 121, WorldId = 11, Name = "asiier"},
            new() {CharacterId = 122, WorldId = 11, Name = "armystrong"},
            new() {CharacterId = 123, WorldId = 11, Name = "brytiaggo"},
        };
    }

    private IEnumerable<CharacterCorrelation> GetCharacterCorrelations()
    {
        return new List<CharacterCorrelation>
        {
            new() { LoginCharacterId = 120, LogoutCharacterId = 123, NumberOfMatches = 2},
            new() { LoginCharacterId = 121, LogoutCharacterId = 122, NumberOfMatches = 3},
            new() { LoginCharacterId = 123, LogoutCharacterId = 121, NumberOfMatches = 4},
            new() { LoginCharacterId = 123, LogoutCharacterId = 122, NumberOfMatches = 5},
            new() { LoginCharacterId = 122, LogoutCharacterId = 120, NumberOfMatches = 6}
        };
    }
}