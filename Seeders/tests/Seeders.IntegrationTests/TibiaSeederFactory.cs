using CharacterAnalyser;
using CharacterAnalyser.Modules;
using DbCleaner;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

#pragma warning disable CS0618

namespace Seeders.IntegrationTests;

public class TibiaSeederFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "postgres",
                Username = "username",
                Password = "password"
            })
            .WithImage("postgres:14.6")
            .Build();

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        await ClearDatabaseAsync(dbContext);
        await SeedDatabse(dbContext);
    }

    public async Task ClearDatabaseAsync(TibiaCharacterFinderDbContext dbContext)
    {
        var tableNames = dbContext.Model.GetEntityTypes().Select(t => t.GetTableName()).Distinct().ToList();

        foreach (var table in tableNames)
        {
            await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {table} CASCADE");
        }
    }

    private async Task SeedDatabse(TibiaCharacterFinderDbContext dbContext)
    {
        await dbContext.Worlds.AddRangeAsync(GetWorlds());
        await dbContext.WorldScans.AddRangeAsync(GetWorldScans());
        await dbContext.Characters.AddRangeAsync(GetCharacters());
        await dbContext.CharacterCorrelations.AddRangeAsync(GetCharacterCorrelations());
        
        await dbContext.SaveChangesAsync();
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        
        await dbContext.Database.MigrateAsync();
        await SeedDatabse(dbContext);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TibiaCharacterFinderDbContext>));
            if (descriptor != null) 
                services.Remove(descriptor);
            
            services.AddSingleton(Options.Create(new ConnectionStringsSection { PostgreSql = _dbContainer.ConnectionString }));
            services.AddSingleton<CharacterManager>();
            services.AddSingleton<CharacterActionsCleaner>();
            services.AddSingleton<Analyser>();
            services.AddSingleton<CharacterSeeder>();
            services.AddSingleton<CharacterCorrelationSeeder>();
            services.AddSingleton<CharacterCorrelationUpdater>();
            services.AddSingleton<CharacterCorrelationDeleter>();
            services.AddSingleton<Cleaner>();
            services.AddDbContext<TibiaCharacterFinderDbContext>(options => options.UseNpgsql(_dbContainer.ConnectionString).UseSnakeCaseNamingConvention());
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    private IEnumerable<World> GetWorlds()
    {
        return new List<World>
        {
            new() { WorldId = 11, Name = "Adra", IsAvailable = true, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Adra" },
            new() { WorldId = 12, Name = "Bastia", IsAvailable = true, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Bastia" }
        };
    }
    
    private IEnumerable<WorldScan> GetWorldScans()
    {
        return new List<WorldScan>
        {
            new() { WorldScanId = 3217, WorldId = 11, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|burntmeat|corvinusik|enchantement pendulement|ergren|fosani|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third|kiszony boczek|kogren|kusahihishaaa|locke der boss|luan takegreen|magna bajnsahe|miligunus|misik venray schisofremus|never loot|polska gospodarka|psyho carp|pugor thoralisti|rhorvaldee|richie slayer|schuruble|shadgrath|skjutlon|skull wick|stasky|teary here|valen bolaget|wroonek"},
            new() { WorldScanId = 3302, WorldId = 11, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|brytiaggo|corvinusik|enchantement pendulement|ergren|fosani|fresita linda|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third|kusahihishaaa|locke der boss|luan takegreen|magna bajnsahe|miligunus|misik venray schisofremus|never loot|nightfire bolt|polska gospodarka|rhorvaldee|richie slayer|schuruble|shadgrath|skjutlon|skull wick|stasky|stronk elzex|syrux zeria|teary here|valen bolaget|wroonek"},
            new() { WorldScanId = 3387, WorldId = 11, ScanCreateDateTime = new DateTime(2022,11,30,20,33,52, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|belzebubba|braws|brytiaggo|corvinusik|enchantement pendulement|ergren|fosani|fresita linda|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third|kusahihishaaa|leinado|locke der boss|macedonio bona festa|magna bajnsahe|miligunus|misik venray schisofremus|never loot|nightfire bolt|polska gospodarka|rhorvaldee|richie slayer|schuruble|shadgrath|skjutlon|stasky|stronk elzex|teary here|valen bolaget|villelinha deathbringer|wroonek"},
            new() { WorldScanId = 3472, WorldId = 11, ScanCreateDateTime = new DateTime(2022,11,30,20,39,06, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|belzebubba|braws|brytiaggo|corvinusik|elder magicmessy|enchantement pendulement|ergren|fosani|fresita linda|ganancia adra|gnum helmet|guga falido|just mojito|kinaduh|kineador|kiperr the third|kiszony boczek|kusahihishaaa|leinado|luan takegreen|macedonio bona festa|magna bajnsahe|meu mano|miligunus|misik venray schisofremus|never loot|polska gospodarka|rhorvaldee|richie slayer|schuruble|shadgrath|skjutlon|stasky|stronk elzex|tallis on adra|teary here|valen bolaget|wroonek"},
            
            new() { WorldScanId = 3727, WorldId = 11, ScanCreateDateTime = new DateTime(2022,11,30,20,55,02, DateTimeKind.Utc), CharactersOnline = "amdurias|anas hamdan|aphov|armystrong|asiier|belzebubba|braws|brytiaggo|burntmeat|corvinusik|darkia slavika|ed ostry gaz|enchantement pendulement|ergren|firemen kyle|fosani|fresita linda|ganancia adra|gnum helmet|guga falido|just mojito|kilomiesa|kinaduh|kineador|kiperr the third|leinado|lil seeley|locke der boss|luan takegreen|magna bajnsahe|meu mano|miligunus|misik sylesja|navariun|never loot|polska gospodarka|richie slayer|rudy materialista|schuruble|shadgrath|skjutlon|skull wick|stasky|teary here|valen bolaget|wroonek|wufind aulkar"},
            
            
            new() { WorldScanId = 3218, WorldId = 12, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "asfarliga quinpin|biedrzyckap|branson on furia|dax overheat|deldrix leon|drama dilemma|dzokson root|ealenias|fabriciusmg|heell kandy|heilikipotel|helikipotel|hyby dysz|immortal wisnievski|impliin killer|itzz xed|jocke kewk|kasta zklanu|khacwhklzkwed wkilah notargi|kriegsherr epnozz|lattmasken|lawbreaker dorszyk|lawbreaker flapjack|lawbreaker rudy|lawbreaker sonike|lawbreaker zenonz|luira lorethor|lurulululuis|mabami lysh|mado big fighter|markin warborn|martechupetano|morrah|navayaa|oqey|payback zklanu|poliillla|rickard zklanu|same hold|smith mythh|sueco sebbe|sylatria|taitarikan nikoxas|toxic rick sanchez|vovo rozzetti|walnu fungu|welsh paly|wyrzutnia rakietowa bastia|yagsog|yduden|zaedon|zeff roker|zkittellz|zkittelz" },
            new() { WorldScanId = 3303, WorldId = 12, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "asfarliga quinpin|beespy|biedrzyckap|dax overheat|deldrix leon|dogshirt|drama dilemma|dzokson root|ealenias|fabriciusmg|ganaelaw|heell kandy|heilikipotel|helikipotel|hyby dysz|immortal wisnievski|impliin killer|itzz xed|jocke kewk|kaos mest|khacwhklzkwed wkilah notargi|kriegsherr epnozz|lattmasken|lawbreaker dorszyk|lawbreaker flapjack|lawbreaker rudy|lawbreaker sonike|lost sekai|luira lorethor|lurulululuis|mabami lysh|mado big fighter|markin warborn|martechupetano|morrah|mpfour|narks akrus|navayaa|oqey|paralyzer anton|payback zklanu|poliillla|ramaed|rickard zklanu|smith mythh|sueco sebbe|sylatria|tiriomans tuhua|toxic rick sanchez|vovo rozzetti|walnu fungu|welsh paly|wyrzutnia rakietowa bastia|yagsog|zaedon|zeff roker|zkittellz" },
            new() { WorldScanId = 3388, WorldId = 12, ScanCreateDateTime = new DateTime(2022,11,30,20,33,52, DateTimeKind.Utc), CharactersOnline = "asfarliga quinpin|beespy|biedrzyckap|budowlaniec from niemcy|dax overheat|deldrix leon|dogshirt|drama dilemma|dramatic haruspex|dzokson root|ealenias|fabriciusmg|ganaelaw|heell kandy|heilikipotel|helikipotel|hyby dysz|immortal wisnievski|itzz xed|jocke kewk|kaos mest|kenn night|khacwhklzkwed wkilah notargi|kriegsherr epnozz|lawbreaker dorszyk|lawbreaker flapjack|lawbreaker rudy|lawbreaker sonike|lost sekai|luira lorethor|lysy hairless|mado big fighter|markin warborn|martechupetano|morrah|narks akrus|navayaa|oqey|payback zklanu|pepson san|poliillla|ramaed|rickard zklanu|smith mythh|sueco sebbe|sylatria|toxic rick sanchez|vael haengd|vovo rozzetti|walnu fungu|welsh paly|wyrzutnia rakietowa bastia|yagsog|yduden|zaedon|zeff roker|zjadacz moli|zkittellz" },
            new() { WorldScanId = 3473, WorldId = 12, ScanCreateDateTime = new DateTime(2022,11,30,20,39,06, DateTimeKind.Utc), CharactersOnline = "arbalestion|asfarliga quinpin|beespy|biedrzyckap|dax overheat|deldrix leon|dogshirt|drama dilemma|dramatic haruspex|dzokson root|ealenias|fabriciusmg|ganaelaw|heell kandy|heilikipotel|helikipotel|hyby dysz|immortal wisnievski|itzz xed|jocke kewk|johankoo|jontom bomb|kaos mest|kasta zklanu|khacwhklkzwed wkilah notarget|khacwhklzkwed wkilah notargi|lawbreaker flapjack|lawbreaker rudy|lost sekai|luira lorethor|lysy hairless|macke blockar|mado big fighter|markin warborn|martechupetano|mere bastia|morrah|navayaa|oqey|payback zklanu|pepson san|poliillla|ramaed|rickard zklanu|smith mythh|sueco sebbe|sylatria|toxic rick sanchez|vael haengd|vovo rozzetti|walnu fungu|welsh paly|wyrzutnia rakietowa bastia|xexu zklanu|yagsog|yduden|zaedon|zeff roker|zjadacz moli|zkittellz" },
                                                  
            new() { WorldScanId = 3813, WorldId = 12, ScanCreateDateTime = new DateTime(2022,11,30,21,00,18, DateTimeKind.Utc), CharactersOnline = "adam on rag|beespy|bumbi dank|bumbii charls|daleih tagarvisox|dax overheat|deldrix leon|drama dilemma|ealenias|fabriciusmg|heilikipotel|hyby dysz|inte yii|itzz xed|jocke kewk|killermaschine|kriegsherr epnozz|lawbreaker flapjack|mado big fighter|markin warborn|martechupetano|mask zica|morrah|navayaa|niryatonan nielan|notoorious|oqey|payback zklanu|poliillla|rickard zklanu|rodrygicius junior|shooper sdeczek|sueco baaz|sueco sebbe|sylatria|touchless frontlane|touchz|uncle pawlik|vael haengd|veduka|vovo rozzetti|walnu fungu|welsh paly|yagsog|zack the legend|zaedon|zeff roker" },
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
            new() { LoginCharacterId = 120, LogoutCharacterId = 122, NumberOfMatches = 3},
            new() { LoginCharacterId = 120, LogoutCharacterId = 123, NumberOfMatches = 6}
        };
    }
}