using MediatR;
using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Catalog.Application.Services;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.Repositories;

namespace Pereprodai.Catalog.Application.Queries.GetAd;

public class GetAdQueryHandler : IRequestHandler<GetAdQuery, AdResponse>
{
    private readonly IAdRepository _adRepository;
    private readonly IViewCountService _viewCountService;

    public GetAdQueryHandler(IAdRepository adRepository, IViewCountService viewCountService)
    {
        _adRepository = adRepository;
        _viewCountService = viewCountService;
    }

    public async Task<AdResponse> Handle(GetAdQuery request, CancellationToken cancellationToken)
    {
        var ad = await _adRepository.GetByIdAsync(request.AdId, cancellationToken);
        if(ad is null || ad.Status is not AdStatus.Published and not AdStatus.Archived && request.RequestingUserId != ad.UserId)
            throw new KeyNotFoundException();
        var views = await _viewCountService.GetViewCountAsync(ad.Id);
        return AdMapper.ToResponse(ad, views);
    }
}
