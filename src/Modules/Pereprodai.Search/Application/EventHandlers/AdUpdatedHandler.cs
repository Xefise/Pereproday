using MediatR;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Search.Infrastructure.Constants;
using Pereprodai.Shared.Domain.Events.Catalog;
using Pereprodai.Shared.Infrastructure.Services.Cache;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdUpdatedHandler : INotificationHandler<AdUpdatedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ICacheService _cacheService;

    public AdUpdatedHandler(IElasticsearchService elasticsearchService, ICacheService cacheService)
    {
        _elasticsearchService = elasticsearchService;
        _cacheService = cacheService;
    }

    public async Task Handle(AdUpdatedEvent notification, CancellationToken ct)
    {
        await _elasticsearchService.DeleteDocumentAsync(notification.Snapshot.AdId, ct);

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Search, ct);
    }
}