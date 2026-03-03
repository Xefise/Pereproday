using Pereprodai.Shared.Domain.Enums;
using Pereprodai.Shared.Application;

namespace Pereprodai.Catalog.Application.Commands.CreateAd;

public record CreateAdCommand(
    Guid UserId,
    string Title,
    string Description,
    string Category,
    decimal PriceAmount,
    Currency PriceCurrency,
    string City,
    string Phone,
    string? Email) : ICommand<Guid>;
