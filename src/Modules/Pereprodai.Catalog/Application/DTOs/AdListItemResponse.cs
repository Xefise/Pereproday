using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Catalog.Application.DTOs;

public record AdListItemResponse(
    Guid Id,
    string Title,
    string Category,
    decimal PriceAmount,
    Currency PriceCurrency,
    string City,
    AdStatus Status,
    DateTime CreatedAt);
