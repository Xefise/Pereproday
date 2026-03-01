using MediatR;
using Pereprodai.Catalog.Domain.Repositories;

namespace Pereprodai.Catalog.Application.Commands.SubmitForModeration;

public class SubmitForModerationCommandHandler : IRequestHandler<SubmitForModerationCommand>
{
    private readonly IAdRepository _adRepository;

    public SubmitForModerationCommandHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task Handle(SubmitForModerationCommand request, CancellationToken cancellationToken)
    {
        var ad = await _adRepository.GetByIdAsync(request.AdId, cancellationToken);
        if(ad is null) throw new KeyNotFoundException();
        ad.EnsureOwner(request.UserId);
        ad.SubmitForModeration();
        await _adRepository.SaveChangesAsync(cancellationToken);
    }
}
