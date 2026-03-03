using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Application;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Catalog.Application.Queries.GetMyAds;

public record GetMyAdsQuery(
    Guid UserId,
    AdStatus? StatusFilter,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResponse<AdListItemResponse>>;
