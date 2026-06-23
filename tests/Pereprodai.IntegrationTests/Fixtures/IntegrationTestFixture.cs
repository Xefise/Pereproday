using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.Moderation;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Search;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Application.Behaviors;
using Pereprodai.Shared.Infrastructure.Services.Cache;
using StackExchange.Redis;
using Testcontainers.Elasticsearch;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Pereprodai.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private readonly ElasticsearchContainer _elasticsearch = new ElasticsearchBuilder()
        .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.15.0")
        .WithEnvironment("xpack.security.enabled", "false")
        .WithEnvironment("xpack.security.http.ssl.enabled", "false")
        .WithEnvironment("xpack.security.transport.ssl.enabled", "false")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgres.StartAsync(),
            _elasticsearch.StartAsync(),
            _redis.StartAsync()
        );
        var connectionString = _postgres.GetConnectionString();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:CatalogDb"] = connectionString,
                ["ConnectionStrings:ModerationDb"] = connectionString,
                ["Elasticsearch:Url"] = $"http://{_elasticsearch.Hostname}:{_elasticsearch.GetMappedPublicPort(9200)}",
                ["Redis:ConnectionString"] = _redis.GetConnectionString()
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCatalogModule(config);
        services.AddModerationModule(config);
        services.AddSearchModule(config);

        var redisConnectionString = config["Redis:ConnectionString"]!;
        services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(await ConnectionMultiplexer.ConnectAsync(redisConnectionString));
        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(CatalogModuleRegistration).Assembly,
                typeof(ModerationModuleRegistration).Assembly,
                typeof(SearchModuleRegistration).Assembly
            );
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(RateLimitingBehavior<,>));
            cfg.AddOpenBehavior(typeof(CacheBehavior<,>));
        });

        Services = services.BuildServiceProvider();

        using var scope = Services.CreateScope();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await catalogDb.Database.MigrateAsync();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        await moderationDb.Database.MigrateAsync();
        var searchService = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
        await searchService.CreateIndexIfNotExistsAsync();


    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.DisposeAsync().AsTask(),
            _elasticsearch.DisposeAsync().AsTask(),
            _redis.DisposeAsync().AsTask()
        );
    }
}