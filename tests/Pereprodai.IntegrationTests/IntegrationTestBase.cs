using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Commands.CreateAd;
using Pereprodai.Catalog.Application.Commands.SubmitForModeration;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.IntegrationTests.Fixtures;
using Pereprodai.Moderation.Application.Commands.ApproveModerationTask;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.IntegrationTests;

[Collection("Integration")]
public abstract class IntegrationTestBase
{
    protected readonly IntegrationTestFixture Fixture;

    protected IntegrationTestBase(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
    }

    protected IServiceScope CreateScope() => Fixture.Services.CreateScope();

    protected async Task<Guid> CreateDraftAd(IMediator mediator, Guid userId,
        string title = "Title", string description = "Description", string category = "Category")
    {
        return await mediator.Send(new CreateAdCommand(userId, title, description, category,
            100, Currency.RUB, "City", "+79871231212", "email@gmail.com"));
    }

    protected async Task<Guid> CreateAndPublishAd(IMediator mediator, ModerationDbContext moderationDb,
        Guid userId, Guid moderatorId,
        string title = "Title", string description = "Description", string category = "Category")
    {
        var adId = await CreateDraftAd(mediator, userId, title, description, category);
        await mediator.Send(new SubmitForModerationCommand(adId, userId));
        var task = await moderationDb.ModerationTasks.FirstAsync(x => x.AdId == adId && x.Status == ModerationStatus.Pending);
        await mediator.Send(new ApproveModerationTaskCommand(task.Id, moderatorId));
        return adId;
    }
}