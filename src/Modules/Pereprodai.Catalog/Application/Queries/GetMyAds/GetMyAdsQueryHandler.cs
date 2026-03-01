using MediatR;
using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Catalog.Domain.Repositories;

namespace Pereprodai.Catalog.Application.Queries.GetMyAds;

public class GetMyAdsQueryHandler : IRequestHandler<GetMyAdsQuery, PagedResponse<AdListItemResponse>>
{
    private readonly IAdRepository _adRepository;

    public GetMyAdsQueryHandler(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task<PagedResponse<AdListItemResponse>> Handle(GetMyAdsQuery request, CancellationToken cancellationToken)
    {
        var ads = await _adRepository.GetByUserIdAsync(
            request.UserId, request.StatusFilter, request.Page, request.PageSize, cancellationToken);
        var dtos = ads.Items.Select(AdMapper.ToListItemResponse).ToList();

        return new PagedResponse<AdListItemResponse>(dtos, ads.TotalCount, request.Page, request.PageSize);
    }
}
