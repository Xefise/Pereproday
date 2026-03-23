using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Commands.UpdateAd;
using Pereprodai.Catalog.Application.Queries.GetMyAds;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.IntegrationTests.Fixtures;
using Pereprodai.Moderation.Application.Commands.RejectModerationTask;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.IntegrationTests;

public class ModerationFlowTests : IntegrationTestBase
{
    public ModerationFlowTests(IntegrationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateAd_ShouldNotCreateModerationTask()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var userId = Guid.NewGuid();

        var adId = await CreateDraftAd(mediator, userId);

        var any = await moderationDb.ModerationTasks.AsNoTracking().AnyAsync(x => x.AdId == adId);
        any.Should().BeFalse();
    }

    [Fact]
    public async Task SubmitAndApprove_ShouldPublishAd()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId);

        var ad = await catalogDb.Ads.AsNoTracking().FirstAsync(x => x.Id == adId);
        ad.Status.Should().Be(AdStatus.Published);
    }

    [Fact]
    public async Task UpdatePublished_ShouldCreateNewModerationTask()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId);

        const string newTitle = "New Title";
        await mediator.Send(new UpdateAdCommand(adId, userId, newTitle, "New Description", "New Category",
            100, Currency.RUB, "Ekb", "+1234567890", "email@gov.gov"));

        var ad = await catalogDb.Ads.AsNoTracking().FirstAsync(x => x.Id == adId);
        ad.Status.Should().Be(AdStatus.OnModeration);

        var adReadModel = await moderationDb.AdReadModels.AsNoTracking().FirstAsync(x => x.AdId == adId);
        adReadModel.Title.Should().Be(newTitle);

        var tasks = await moderationDb.ModerationTasks.AsNoTracking()
            .Where(x => x.AdId == adId).ToListAsync();
        tasks.Should().HaveCount(2);
        tasks.Should().ContainSingle(x => x.Status == ModerationStatus.Approved);
        tasks.Should().ContainSingle(x => x.Status == ModerationStatus.Pending);
    }

    [Fact]
    public async Task RejectAfterUpdate_ShouldRejectAd()
    {
        using var scope = CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var moderationDb = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateAndPublishAd(mediator, moderationDb, userId, moderatorId);

        await mediator.Send(new UpdateAdCommand(adId, userId, "Updated", "Desc", "Cat",
            100, Currency.RUB, "Ekb", "+1234567890", "email@gov.gov"));

        var pendingTask = await moderationDb.ModerationTasks.FirstAsync(
            x => x.AdId == adId && x.Status == ModerationStatus.Pending);

        const string reason = "An reason";
        await mediator.Send(new RejectModerationTaskCommand(pendingTask.Id, moderatorId, reason));

        var rejectedTask = await moderationDb.ModerationTasks.AsNoTracking()
            .FirstAsync(x => x.Id == pendingTask.Id);
        rejectedTask.Status.Should().Be(ModerationStatus.Rejected);
        rejectedTask.RejectionReason.Should().Be(reason);

        var userAds = await mediator.Send(new GetMyAdsQuery(userId, null, 1, 10));
        userAds.Items.Should().ContainSingle().Which.Status.Should().Be(AdStatus.Rejected);
    }
}
