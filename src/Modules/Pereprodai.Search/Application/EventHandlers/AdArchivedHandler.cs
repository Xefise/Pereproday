using MediatR;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Search.Infrastructure.Constants;
using Pereprodai.Shared.Domain.Events.Catalog;
using Pereprodai.Shared.Infrastructure.Services.Cache;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdArchivedHandler : INotificationHandler<AdArchivedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ICacheService _cacheService;

    public AdArchivedHandler(IElasticsearchService elasticsearchService, ICacheService cacheService)
    {
        _elasticsearchService = elasticsearchService;
        _cacheService = cacheService;
    }

    public async Task Handle(AdArchivedEvent notification, CancellationToken ct)
    {
        await _elasticsearchService.DeleteDocumentAsync(notification.AdId, ct);

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Search, ct);
    }
}