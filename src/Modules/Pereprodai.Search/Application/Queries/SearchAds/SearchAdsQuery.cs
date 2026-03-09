using Pereprodai.Search.Infrastructure.Constants;
using Pereprodai.Shared.Application;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Search.Application.Queries.SearchAds;

public record SearchAdsQuery(
    string? SearchString, string? Category, string? City, decimal? PriceFrom, decimal? PriceTo, string? Sort,
    Currency Currency = Currency.RUB, int Page = 1, int PageSize = 20
) : IQuery<SearchResult>, ICacheable
{
    public string CachePrefix => CacheKeys.Search;
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}