using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Commands.ArchiveAd;
using Pereprodai.IntegrationTests.Fixtures;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Search.Application.Queries.SearchAds;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Search.Infrastructure.Constants;
using StackExchange.Redis;

namespace Pereprodai.IntegrationTests;

public class SearchFlowTests : IntegrationTestBase
{
    public SearchFlowTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task PublishedAd_ShouldBeFoundInSearch()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var es = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId,
            title: "Велосипед горный", description: "Отличный велосипед для гор", category: "Транспорт");

        await es.RefreshIndexAsync();

        var result = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        result.Ads.Items.Should().ContainSingle(x => x.Id == adId);
        result.Ads.Items.First().Title.Should().Be("Велосипед горный");
    }

    [Fact]
    public async Task ArchivedAd_ShouldNotBeFoundInSearch()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var es = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId,
            title: "Велосипед горный", description: "Отличный велосипед для гор", category: "Транспорт");

        await es.RefreshIndexAsync();

        var afterPublishResult = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        afterPublishResult.Ads.Items.Should().ContainSingle(x => x.Id == adId);
        afterPublishResult.Ads.Items.First().Title.Should().Be("Велосипед горный");

        await mediator.Send(new ArchiveAdCommand(adId, userId));
        await es.RefreshIndexAsync();

        var afterArchiveResult = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        afterArchiveResult.Ads.Items.Should().NotContain(x => x.Id == adId);
    }

    [Fact]
    public async Task SearchResults_ShouldBeCachedInRedis()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var es = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var server = redis.GetServers().First();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId,
            title: "Велосипед горный", description: "Отличный велосипед для гор", category: "Транспорт");

        await es.RefreshIndexAsync();

        var afterPublishResult = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        afterPublishResult.Ads.Items.Should().ContainSingle(x => x.Id == adId);
        afterPublishResult.Ads.Items.First().Title.Should().Be("Велосипед горный");

        var keys = server.Keys(pattern: $"{CacheKeys.Search}:*").ToList();
        keys.Should().NotBeEmpty("Первый запрос должен был закешироваться");

        var afterPublishResultCache = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        afterPublishResultCache.Ads.Items.Should().ContainSingle(x => x.Id == adId);
        afterPublishResultCache.Ads.Items.First().Title.Should().Be("Велосипед горный");

        var ad2Id = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId,
            title: "Велосипед городской", description: "Отличный велосипед для города", category: "Транспорт");

        var keysAfterPublish = server.Keys(pattern: $"{CacheKeys.Search}:*").ToList();
        keysAfterPublish.Should().BeEmpty("publish должен инвалидировать кеш");

        await es.RefreshIndexAsync();

        var afterPublish2NdResult = await mediator.Send(new SearchAdsQuery("велосипед", null, null, null, null, null));

        afterPublish2NdResult.Ads.Items.Should().Contain(x => x.Id == adId);
        afterPublish2NdResult.Ads.Items.Should().Contain(x => x.Id == ad2Id);
    }
}
