using MediatR;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Shared.Domain.Events.Moderation;

namespace Pereprodai.Catalog.Application.EventHandlers;

public class ModerationTaskApprovedHandler : INotificationHandler<ModerationTaskApprovedEvent>
{
    private readonly IAdRepository _adRepository;

    public ModerationTaskApprovedHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task Handle(ModerationTaskApprovedEvent notification, CancellationToken ct)
    {
        var ad = await _adRepository.GetByIdAsync(notification.AdId, ct);
        if(ad == null) throw new KeyNotFoundException();

        ad.Publish();
        await _adRepository.SaveChangesAsync(ct);
    }
}