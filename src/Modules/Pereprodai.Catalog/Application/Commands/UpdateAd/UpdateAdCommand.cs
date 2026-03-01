using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Application;

namespace Pereprodai.Catalog.Application.Commands.UpdateAd;

public record UpdateAdCommand(
    Guid AdId,
    Guid UserId,
    string Title,
    string Description,
    string Category,
    decimal PriceAmount,
    Currency PriceCurrency,
    string City,
    string Phone,
    string? Email) : ICommand;
