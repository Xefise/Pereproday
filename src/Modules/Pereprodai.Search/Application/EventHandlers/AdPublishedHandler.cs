using MediatR;
using Pereprodai.Search.Documents;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Search.Application.EventHandlers;

public class AdPublishedHandler : INotificationHandler<AdPublishedEvent>
{
    private readonly IElasticsearchService _elasticsearchService;

    public AdPublishedHandler(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
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
    }
}