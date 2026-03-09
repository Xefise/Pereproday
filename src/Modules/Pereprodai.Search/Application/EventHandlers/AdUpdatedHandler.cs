using MediatR;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdUpdatedHandler : INotificationHandler<AdUpdatedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;

    public AdUpdatedHandler(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    public async Task Handle(AdUpdatedEvent notification, CancellationToken ct)
    {
        await _elasticsearchService.DeleteDocumentAsync(notification.Snapshot.AdId, ct);
    }
}