using MediatR;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Catalog.Domain.ValueObjects;

namespace Pereprodai.Catalog.Application.Commands.CreateAd;

public class CreateAdCommandHandler : IRequestHandler<CreateAdCommand, Guid>
{
    private readonly IAdRepository _adRepository;

    public CreateAdCommandHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task<Guid> Handle(CreateAdCommand request, CancellationToken cancellationToken)
    {
        var ad = Ad.Create(request.UserId, request.Title, request.Description, request.Category,
            new Price(request.PriceAmount, request.PriceCurrency),
            new Location(request.City),
            new ContactInfo(request.Phone, request.Email)
        );
        await _adRepository.AddAsync(ad, cancellationToken);
        await _adRepository.SaveChangesAsync(cancellationToken);
        return ad.Id;
    }
}
