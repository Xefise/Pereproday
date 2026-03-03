using MediatR;
using Microsoft.EntityFrameworkCore;
using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Domain.Repositories;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Moderation.Infrastructure.ReadModels;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Moderation.Application.EventHandlers;

public class AdSubmittedForModerationHandler : INotificationHandler<AdSubmittedForModerationEvent>
{
    private readonly ModerationDbContext _context;
    private readonly IModerationTaskRepository _moderationTaskRepository;

    public AdSubmittedForModerationHandler(ModerationDbContext context, IModerationTaskRepository moderationTaskRepository)
    {
        _context = context;
        _moderationTaskRepository = moderationTaskRepository;
    }

    public async Task Handle(AdSubmittedForModerationEvent notification, CancellationToken ct)
    {
        var moderationTask = ModerationTask.Create(notification.Snapshot.AdId);

        await _moderationTaskRepository.AddAsync(moderationTask, ct);

        var adReadModel = await _context.AdReadModels.FirstOrDefaultAsync(x => x.AdId == notification.Snapshot.AdId, ct);
        if (adReadModel != null)
        {
            adReadModel.Update(notification.Snapshot);
        }
        else
        {
            adReadModel = new AdReadModel(notification.Snapshot);
            await _context.AdReadModels.AddAsync(adReadModel, ct);
        }

        await _context.SaveChangesAsync(ct);
    }
}