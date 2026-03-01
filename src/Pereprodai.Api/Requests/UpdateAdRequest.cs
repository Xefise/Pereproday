using Pereprodai.Catalog.Domain.Enums;

namespace Pereprodai.Api.Requests;

public record UpdateAdRequest(
    string Title,
    string Description,
    string Category,
    decimal PriceAmount,
    Currency PriceCurrency,
    string City,
    string Phone,
    string? Email);
