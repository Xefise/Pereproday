using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

public record AdSnapshot(
    Guid AdId, Guid UserId, string Title, string Description,
    string Category, decimal PriceAmount, Currency PriceCurrency,
    string LocationCity, string ContactInfoPhone, string? ContactInfoEmail,
    DateTime? CreatedAt = null, DateTime? UpdatedAt = null);