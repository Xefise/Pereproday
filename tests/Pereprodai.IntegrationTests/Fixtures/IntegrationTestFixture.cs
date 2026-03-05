using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.Moderation;
using Pereprodai.Moderation.Infrastructure;
using Testcontainers.PostgreSql;

namespace Pereprodai.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var connectionString = _postgres.GetConnectionString();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:CatalogDb"] = connectionString,
                ["ConnectionStrings:ModerationDb"] = connectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCatalogModule(config);
        services.AddModerationModule(config);

        Services = services.BuildServiceProvider();

        using var scope = Services.CreateScope();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await catalogDb.Database.MigrateAsync();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        await moderationDb.Database.MigrateAsync();


    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}