using MediatR;
using Microsoft.EntityFrameworkCore;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Moderation.Application.EventHandlers;

public class AdUpdatedHandler : INotificationHandler<AdUpdatedEvent>
{
    private readonly ModerationDbContext _context;

    public AdUpdatedHandler(ModerationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AdUpdatedEvent notification, CancellationToken ct)
    {
        var adReadModel = await _context.AdReadModels.FirstOrDefaultAsync(x => x.AdId == notification.Snapshot.AdId, ct);
        if (adReadModel != null)
        {
            adReadModel.Update(notification.Snapshot);
        }

        await _context.SaveChangesAsync(ct);
    }
}