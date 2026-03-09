using MediatR;
using Pereprodai.Search.Infrastructure;

namespace Pereprodai.Search.Application.Queries.SearchAds;

public class SearchAdsQueryHandler : IRequestHandler<SearchAdsQuery, SearchResult>
{
    private readonly IElasticsearchService _elasticsearchService;

    public SearchAdsQueryHandler(IElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    public async Task<SearchResult> Handle(SearchAdsQuery query, CancellationToken ct)
    {
        var clampedQuery = query with { PageSize = Math.Clamp(query.PageSize, 1, 50), Page = Math.Max(query.Page, 1) };
        return await _elasticsearchService.SearchAsync(clampedQuery, ct);
    }
}