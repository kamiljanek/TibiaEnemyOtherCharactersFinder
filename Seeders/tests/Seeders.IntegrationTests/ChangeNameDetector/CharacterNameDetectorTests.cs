using System.Text;
using ChangeNameDetector;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMq.Conventions;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.ChangeNameDetector;

[Collection("Seeder test collection")]
public class CharacterNameDetectorTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly Mock<ITibiaDataService> _tibiaDataServiceMock = new();

    public CharacterNameDetectorTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task Run_WhenCharacterExistAndWasNotTraded_ShouldOnlyUpdateVerifiedDate()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContextForMock = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChangeNameDetectorService>>();
        var busPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        var charactersBeforeDetector = dbContextForMock.Characters.AsNoTracking().ToList();

        foreach (var character in charactersBeforeDetector)
        {
            _tibiaDataServiceMock.Setup(r => r.FetchCharacter(character.Name)).ReturnsAsync(PrepareExistingTibiaDataCharacter(character.Name));
        }

        var changeNameDetector = new ChangeNameDetectorService(logger, repository, _tibiaDataServiceMock.Object, busPublisher);


        // Act
        await changeNameDetector.Run();


        // Assert
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterDetector = dbContext.Characters.AsNoTracking().ToList();

        charactersAfterDetector.Select(c => c.VerifiedDate).Should().AllBeEquivalentTo(DateOnly.FromDateTime(DateTime.Now));
        charactersAfterDetector.Count.Should().Be(4);
        charactersBeforeDetector.Select(c => c.VerifiedDate).Should().OnlyContain(date => date == null);
    }

    [Fact]
    public async Task Run_WhenCharacterNotExist_ShouldSendEventDeleteCharacterWithCorrelationsEvent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContextForMock = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChangeNameDetectorService>>();
        var busPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        var charactersBeforeDetector = dbContextForMock.Characters.AsNoTracking().ToList();

        for (var i = 0; i < charactersBeforeDetector.Count; i++)
        {
            SetupTibiaDataServiceMock(charactersBeforeDetector[i].Name, (i < 2, PrepareNonExistingTibiaDataCharacter));
        }

        var changeNameDetector = new ChangeNameDetectorService(logger, repository, _tibiaDataServiceMock.Object, busPublisher);


        // Act
        await changeNameDetector.Run();


        // Assert
        var receivedObjects = SubscribeRabbitMessagesFromQueue<DeleteCharacterWithCorrelationsEvent>();

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterDetector = dbContext.Characters.AsNoTracking().ToList();


        charactersAfterDetector.Select(c => c.VerifiedDate).Should().AllBeEquivalentTo(DateOnly.FromDateTime(DateTime.Now));
        charactersAfterDetector.Count.Should().Be(4);
        charactersBeforeDetector.Select(c => c.VerifiedDate).Should().OnlyContain(date => date == null);

        receivedObjects.Select(o => o.CharacterId).Should().Contain(new[] { 120, 121 });
        receivedObjects.Count.Should().Be(2);
    }

    [Fact]
    public async Task Run_WhenCharacterWasTraded_ShouldSendEventDeleteChcaracterCorrelationsEvent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContextForMock = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChangeNameDetectorService>>();
        var busPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        var charactersBeforeDetector = dbContextForMock.Characters.AsNoTracking().ToList();

        for (var i = 0; i < charactersBeforeDetector.Count; i++)
        {
            SetupTibiaDataServiceMock(charactersBeforeDetector[i].Name, (i < 2, () => PrepareTradedTibiaDataCharacter(charactersBeforeDetector[i].Name)));
        }

        var changeNameDetector = new ChangeNameDetectorService(logger, repository, _tibiaDataServiceMock.Object, busPublisher);


        // Act
        await changeNameDetector.Run();


        // Assert
        var receivedObjects = SubscribeRabbitMessagesFromQueue<DeleteCharacterCorrelationsEvent>();

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterDetector = dbContext.Characters.AsNoTracking().ToList();


        charactersAfterDetector.Select(c => c.VerifiedDate).Should().AllBeEquivalentTo(DateOnly.FromDateTime(DateTime.Now));
        charactersAfterDetector.Count.Should().Be(4);
        charactersBeforeDetector.Select(c => c.VerifiedDate).Should().OnlyContain(date => date == null);

        receivedObjects.Select(o => o.CharacterId).Should().Contain(new[] { 120, 121 });
        receivedObjects.Count.Should().Be(2);
    }

    [Fact]
    public async Task Run_WhenCharacterWasFoundInFormerNames_ShouldSendEventMergeTwoCharactersEvent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContextForMock = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChangeNameDetectorService>>();
        var busPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        var charactersBeforeDetector = dbContextForMock.Characters.AsNoTracking().ToList();

        for (var i = 0; i < charactersBeforeDetector.Count; i++)
        {
            SetupTibiaDataServiceMock(charactersBeforeDetector[i].Name, (i < 1, () => PrepareChangedNameCharacter(charactersBeforeDetector[i].Name)));
        }

        var changeNameDetector = new ChangeNameDetectorService(logger, repository, _tibiaDataServiceMock.Object, busPublisher);


        // Act
        await changeNameDetector.Run();


        // Assert
        var receivedObjects = SubscribeRabbitMessagesFromQueue<MergeTwoCharactersEvent>();

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterDetector = dbContext.Characters.AsNoTracking().ToList();


        charactersAfterDetector.Select(c => c.VerifiedDate).Should().AllBeEquivalentTo(DateOnly.FromDateTime(DateTime.Now));
        charactersAfterDetector.Count.Should().Be(4);
        charactersBeforeDetector.Select(c => c.VerifiedDate).Should().OnlyContain(date => date == null);

        receivedObjects[0].OldCharacterId.Should().Be(120);
        receivedObjects[0].NewCharacterId.Should().Be(121);
        receivedObjects.Count.Should().Be(1);
    }

    [Fact]
    public async Task Run_WhenCharacterWasFoundInFormerNamesButNotFoundInDatabase_ShouldOnlyUpdateVerifiedDate()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContextForMock = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChangeNameDetectorService>>();
        var busPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        var charactersBeforeDetector = dbContextForMock.Characters.AsNoTracking().ToList();

        for (var i = 0; i < charactersBeforeDetector.Count; i++)
        {
            SetupTibiaDataServiceMock(charactersBeforeDetector[i].Name,
                (i < 1, () => PrepareChangedNameCharacterWithNameNonExistentInDatabase(charactersBeforeDetector[i].Name)));
        }

        var changeNameDetector = new ChangeNameDetectorService(logger, repository, _tibiaDataServiceMock.Object, busPublisher);


        // Act
        await changeNameDetector.Run();


        // Assert
        var receivedObjects = SubscribeRabbitMessagesFromQueue<MergeTwoCharactersEvent>();

        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var charactersAfterDetector = dbContext.Characters.AsNoTracking().ToList();


        charactersAfterDetector.Select(c => c.VerifiedDate).Should().AllBeEquivalentTo(DateOnly.FromDateTime(DateTime.Now));
        charactersAfterDetector.Count.Should().Be(4);
        charactersBeforeDetector.Select(c => c.VerifiedDate).Should().OnlyContain(date => date == null);

        receivedObjects.Count.Should().Be(0);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private List<T> SubscribeRabbitMessagesFromQueue<T>() where T: IntegrationEvent
    {
        var receivedObjects = new List<T>();

        var options = GetOptions();
        var factory = GetFactory(options);

        using var connection = factory.CreateConnection("tibia-eocf-subscriber");
        using var channel = connection.CreateModel();
        var exchangeOptions = options.Exchange;
        var deadLetterOptions = options.DeadLetter;
        var queueOptions = options.Queue;

        var queueName = $"{exchangeOptions.Name}.{typeof(T).Name.ToRabbitSnakeCase()}";

        Dictionary<string, object> queueArguments = new()
        {
            { "x-dead-letter-exchange", $"{deadLetterOptions.Prefix}{exchangeOptions.Name}" },
            { "x-dead-letter-routing-key", $"{deadLetterOptions.Prefix}{queueName}" }
        };

        channel.QueueDeclare(queue: queueName, queueOptions.Durable, queueOptions.Exclusive, queueOptions.AutoDelete, queueArguments);

        while (true)
        {
            BasicGetResult result = channel.BasicGet(queueName, autoAck: true);

            if (result == null)
                break;

            byte[] receivedBytes = result.Body.ToArray();
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
            receivedObjects.Add(JsonConvert.DeserializeObject<T>(receivedMessage)!);
        }

        channel.Close();
        connection.Close();

        return receivedObjects;
    }

    private RabbitMqSection GetOptions()
    {
        var configuration = _factory.Configuration;

        var section = configuration.GetSection(RabbitMqSection.SectionName);

        var options = section.Get<RabbitMqSection>();

        return options!;
    }

    private ConnectionFactory GetFactory(RabbitMqSection options)
    {
        return new ConnectionFactory
        {
            Uri = new Uri(options!.HostUrl),
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            DispatchConsumersAsync = true
        };
    }

    private void SetupTibiaDataServiceMock(string characterName,
        params (bool Flag, Func<TibiaDataCharacterInformationResult> PrepareFunction)[] preparations)
    {
        var preparation = preparations
            .FirstOrDefault(p => p.Flag);

        var result = preparation.Flag ? preparation.PrepareFunction.Invoke() : PrepareExistingTibiaDataCharacter(characterName);

        _tibiaDataServiceMock
            .Setup(r => r.FetchCharacter(characterName))
            .ReturnsAsync(result);
    }

    private TibiaDataCharacterInformationResult PrepareExistingTibiaDataCharacter(string name)
    {
        return new TibiaDataCharacterInformationResult()
        {
            characters = new CharactersResult()
            {
                character = new CharacterResult()
                {
                    name = name,
                    level = 100,
                    vocation = "Druid",
                    world = "Adra",
                    last_login = "2020-08-31T13:47:00Z",
                    traded = false
                }
            }
        };
    }

    private TibiaDataCharacterInformationResult PrepareNonExistingTibiaDataCharacter()
    {
        return new TibiaDataCharacterInformationResult()
        {
            characters = new CharactersResult()
            {
                character = new CharacterResult()
                {
                    name = "",
                    level = 0,
                    vocation = "",
                    world = "",
                    last_login = "",
                    traded = false
                }
            }
        };
    }

    private TibiaDataCharacterInformationResult PrepareTradedTibiaDataCharacter(string name)
    {
        return new TibiaDataCharacterInformationResult()
        {
            characters = new CharactersResult()
            {
                character = new CharacterResult()
                {
                    name = name,
                    level = 100,
                    vocation = "Druid",
                    world = "Adra",
                    last_login = "2020-08-31T13:47:00Z",
                    traded = true
                }
            }
        };
    }

    private TibiaDataCharacterInformationResult PrepareChangedNameCharacter(string name)
    {
        return new TibiaDataCharacterInformationResult()
        {
            characters = new CharactersResult()
            {
                character = new CharacterResult()
                {
                    name = "asiier",
                    former_names = new List<string>(){ name, "test2" },
                    level = 100,
                    vocation = "Druid",
                    world = "Adra",
                    last_login = "2020-08-31T13:47:00Z",
                    traded = false
                }
            }
        };
    }

    private TibiaDataCharacterInformationResult PrepareChangedNameCharacterWithNameNonExistentInDatabase(string name)
    {
        return new TibiaDataCharacterInformationResult()
        {
            characters = new CharactersResult()
            {
                character = new CharacterResult()
                {
                    name = "test",
                    former_names = new List<string>(){ name, "test2" },
                    level = 100,
                    vocation = "Druid",
                    world = "Adra",
                    last_login = "2020-08-31T13:47:00Z",
                    traded = false
                }
            }
        };
    }
}