using Pereprodai.Search.Documents;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Search.Application.Queries.SearchAds;

public record SearchResult(
    PagedResponse<AdSearchDocument> Ads,
    Dictionary<string, long> CategoryFacets);