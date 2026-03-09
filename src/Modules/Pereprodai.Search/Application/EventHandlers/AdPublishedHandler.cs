using MediatR;
using Pereprodai.Search.Documents;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Search.Infrastructure.Constants;
using Pereprodai.Shared.Domain.Events.Catalog;
using Pereprodai.Shared.Infrastructure.Services.Cache;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdPublishedHandler : INotificationHandler<AdPublishedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ICacheService _cacheService;

    public AdPublishedHandler(IElasticsearchService elasticsearchService, ICacheService cacheService)
    {
        _elasticsearchService = elasticsearchService;
        _cacheService = cacheService;
    }

    public async Task Handle(AdPublishedEvent notification, CancellationToken ct)
    {
        var ad = notification.AdSnapshot;
        await _elasticsearchService.IndexDocumentAsync(new AdSearchDocument()
        {
            Id = ad.AdId,
            UserId = ad.UserId,
            Title = ad.Title,
            Description = ad.Description,
            Category = ad.Category,
            City = ad.LocationCity,
            Price = ad.PriceAmount,
            Currency = ad.PriceCurrency.ToString(),
            Phone = ad.ContactInfoPhone,
            Email = ad.ContactInfoEmail,
            CreatedAt = ad.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = ad.UpdatedAt ?? DateTime.UtcNow,
        }, ct);

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Search, ct);
    }
}