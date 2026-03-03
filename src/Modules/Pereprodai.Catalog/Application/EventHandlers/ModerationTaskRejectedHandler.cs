using MediatR;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Shared.Domain.Events.Moderation;

namespace Pereprodai.Catalog.Application.EventHandlers;

public class ModerationTaskRejectedHandler : INotificationHandler<ModerationTaskRejectedEvent>
{
    private readonly IAdRepository _adRepository;

    public ModerationTaskRejectedHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task Handle(ModerationTaskRejectedEvent notification, CancellationToken ct)
    {
        var ad = await _adRepository.GetByIdAsync(notification.AdId, ct);
        if(ad == null) throw new KeyNotFoundException();

        ad.Reject(notification.Reason);
        await _adRepository.SaveChangesAsync(ct);
    }
}