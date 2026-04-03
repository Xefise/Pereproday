using MediatR;
using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Catalog.Application.Services;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Catalog.Application.Queries.GetMyAds;

public class GetMyAdsQueryHandler : IRequestHandler<GetMyAdsQuery, PagedResponse<AdListItemResponse>>
{
    private readonly IAdRepository _adRepository;
    private readonly IViewCountService _viewCountService;

    public GetMyAdsQueryHandler(IAdRepository adRepository, IViewCountService viewCountService)
    {
        _adRepository = adRepository;
        _viewCountService = viewCountService;
    }

    public async Task<PagedResponse<AdListItemResponse>> Handle(GetMyAdsQuery request, CancellationToken cancellationToken)
    {
        var ads = await _adRepository.GetByUserIdAsync(
            request.UserId, request.StatusFilter, request.Page, request.PageSize, cancellationToken);
        var adsViews = await _viewCountService.GetViewCountsAsync(ads.Items.Select(a => a.Id));
        var dtos = ads.Items.Select(x => AdMapper.ToListItemResponse(x, adsViews.GetValueOrDefault(x.Id, 0))).ToList();

        return new PagedResponse<AdListItemResponse>(dtos, ads.TotalCount, request.Page, request.PageSize);
    }
}
