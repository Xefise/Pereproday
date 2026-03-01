using MediatR;
using Pereprodai.Catalog.Domain.Repositories;

namespace Pereprodai.Catalog.Application.Commands.ArchiveAd;

public class ArchiveAdCommandHandler : IRequestHandler<ArchiveAdCommand>
{
    private readonly IAdRepository _adRepository;

    public ArchiveAdCommandHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task Handle(ArchiveAdCommand request, CancellationToken cancellationToken)
    {
        var ad = await _adRepository.GetByIdAsync(request.AdId, cancellationToken);
        if(ad is null) throw new KeyNotFoundException();
        ad.EnsureOwner(request.UserId);
        ad.Archive();
        await _adRepository.SaveChangesAsync(cancellationToken);
    }
}
