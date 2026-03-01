using MediatR;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Catalog.Domain.ValueObjects;

namespace Pereprodai.Catalog.Application.Commands.UpdateAd;

public class UpdateAdCommandHandler : IRequestHandler<UpdateAdCommand>
{
    private readonly IAdRepository _adRepository;

    public UpdateAdCommandHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task Handle(UpdateAdCommand request, CancellationToken cancellationToken)
    {
        var ad = await _adRepository.GetByIdAsync(request.AdId, cancellationToken);
        if(ad is null) throw new KeyNotFoundException();
        ad.EnsureOwner(request.UserId);
        ad.Update(request.Title, request.Description, request.Category,
            new Price(request.PriceAmount, request.PriceCurrency),
            new Location(request.City),
            new ContactInfo(request.Phone, request.Email)
        );
        await _adRepository.SaveChangesAsync(cancellationToken);
    }
}
