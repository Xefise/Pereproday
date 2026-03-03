using Pereprodai.Shared.Application;
using Pereprodai.Shared.Domain.Enums;

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
