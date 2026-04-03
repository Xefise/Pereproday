using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Catalog.Application.DTOs;

public record AdResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    string Category,
    decimal PriceAmount,
    Currency PriceCurrency,
    string City,
    string Phone,
    string? Email,
    AdStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    long Views);
