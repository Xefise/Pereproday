using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Commands.CreateAd;
using Pereprodai.Catalog.Application.Commands.SubmitForModeration;
using Pereprodai.Catalog.Application.Commands.UpdateAd;
using Pereprodai.Catalog.Application.Queries.GetMyAds;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.IntegrationTests.Fixtures;
using Pereprodai.Moderation.Application.Commands.ApproveModerationTask;
using Pereprodai.Moderation.Application.Commands.RejectModerationTask;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.IntegrationTests;

public class ModerationFlowTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public ModerationFlowTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(IMediator mediator, CatalogDbContext catalogDb, ModerationDbContext moderationDb)> CreateScope()
    {
        var scope = _fixture.Services.CreateScope();
        return (
            scope.ServiceProvider.GetRequiredService<IMediator>(),
            scope.ServiceProvider.GetRequiredService<CatalogDbContext>(),
            scope.ServiceProvider.GetRequiredService<ModerationDbContext>()
        );
    }

    private async Task<Guid> CreateDraftAd(IMediator mediator, Guid userId)
    {
        return await mediator.Send(new CreateAdCommand(userId, "Title", "Description", "Category",
            100, Currency.RUB, "City", "+79871231212", "email@gmail.com"));
    }

    private async Task<Guid> SubmitAndApprove(IMediator mediator, ModerationDbContext moderationDb, Guid adId, Guid userId, Guid moderatorId)
    {
        await mediator.Send(new SubmitForModerationCommand(adId, userId));
        var task = await moderationDb.ModerationTasks.FirstAsync(x => x.AdId == adId && x.Status == ModerationStatus.Pending);
        await mediator.Send(new ApproveModerationTaskCommand(task.Id, moderatorId));
        return task.Id;
    }

    [Fact]
    public async Task CreateAd_ShouldNotCreateModerationTask()
    {
        var (mediator, catalogDb, moderationDb) = await CreateScope();
        var userId = Guid.NewGuid();

        var adId = await CreateDraftAd(mediator, userId);

        var any = await moderationDb.ModerationTasks.AsNoTracking().AnyAsync(x => x.AdId == adId);
        any.Should().BeFalse();
    }

    [Fact]
    public async Task SubmitAndApprove_ShouldPublishAd()
    {
        var (mediator, catalogDb, moderationDb) = await CreateScope();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateDraftAd(mediator, userId);
        await SubmitAndApprove(mediator, moderationDb, adId, userId, moderatorId);

        var ad = await catalogDb.Ads.AsNoTracking().FirstAsync(x => x.Id == adId);
        ad.Status.Should().Be(AdStatus.Published);
    }

    [Fact]
    public async Task UpdatePublished_ShouldCreateNewModerationTask()
    {
        var (mediator, catalogDb, moderationDb) = await CreateScope();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateDraftAd(mediator, userId);
        await SubmitAndApprove(mediator, moderationDb, adId, userId, moderatorId);

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
        var (mediator, catalogDb, moderationDb) = await CreateScope();
        var userId = Guid.NewGuid();
        var moderatorId = Guid.NewGuid();

        var adId = await CreateDraftAd(mediator, userId);
        await SubmitAndApprove(mediator, moderationDb, adId, userId, moderatorId);

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
