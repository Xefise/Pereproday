using MediatR;
using Pereprodai.Search.Documents;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdArchivedHandler : INotificationHandler<AdArchivedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;

    public AdArchivedHandler(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    public async Task Handle(AdArchivedEvent notification, CancellationToken ct)
    {
        await _elasticsearchService.DeleteDocumentAsync(notification.AdId, ct);
    }
}