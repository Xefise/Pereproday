using Pereprodai.Search.Application.Queries.SearchAds;
using Pereprodai.Search.Documents;

namespace Pereprodai.Search.Infrastructure;

public interface IElasticsearchService
{
    Task<SearchResult> SearchAsync(SearchAdsQuery query, CancellationToken ct = default);
    Task CreateIndexIfNotExistsAsync(CancellationToken ct = default);
    Task IndexDocumentAsync(AdSearchDocument document, CancellationToken ct = default);
    Task DeleteDocumentAsync(Guid id, CancellationToken ct = default);
}