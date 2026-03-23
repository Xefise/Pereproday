using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Pereprodai.Search.Application.Queries.SearchAds;
using Pereprodai.Search.Documents;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Search.Infrastructure;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string IndexName = "ads";

    public ElasticsearchService(ElasticsearchClient client, ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<SearchResult> SearchAsync(SearchAdsQuery query, CancellationToken ct = default)
    {
        var filters = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.Category))
            filters.Add(new TermQuery(new Field("category")) { Value = query.Category });

        if (!string.IsNullOrWhiteSpace(query.City))
            filters.Add(new TermQuery(new Field("city")) { Value = query.City });

        filters.Add(new TermQuery(new Field("currency")) { Value = query.Currency.ToString() });

        if (query.PriceFrom.HasValue || query.PriceTo.HasValue)
        {
            filters.Add(new NumberRangeQuery(new Field("price")) // todo потом надо что-то делать с валютой
            {
                Gte = query.PriceFrom.HasValue ? (double)query.PriceFrom.Value : null,
                Lte = query.PriceTo.HasValue ? (double)query.PriceTo.Value : null
            });
        }

        var response = await _client.SearchAsync<AdSearchDocument>(s => s
            .Index(IndexName)
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .Bool(b =>
                {
                    if (!string.IsNullOrWhiteSpace(query.SearchString))
                    {
                        b.Must(m => m
                            .MultiMatch(mm => mm
                                .Query(query.SearchString)
                                .Fields(new[] { "title^3", "description" }) // title x3
                                .Fuzziness(new Fuzziness("AUTO"))          // опечатки
                            )
                        );
                    }

                    // Filter — фильтры без влияния на score
                    if (filters.Count > 0)
                        b.Filter(filters.ToArray());
                })
            )
            .Sort(query.Sort switch
            {
                "price_asc" => so => so.Field(f => f.Price, new FieldSort { Order = SortOrder.Asc }),
                "price_desc" => so => so.Field(f => f.Price, new FieldSort { Order = SortOrder.Desc }),
                "date" => so => so.Field(f => f.UpdatedAt, new FieldSort { Order = SortOrder.Desc }),
                _ => so => so.Field(f => f.UpdatedAt, new FieldSort { Order = SortOrder.Desc }),
                //_ => so => so.Score(new ScoreSort { Order = SortOrder.Desc })
            })
            .Aggregations(agg => agg
                .Add("categories", a => a
                    .Terms(t => t
                        .Field(f => f.Category)
                        .Size(20)
                    )
                )
            ), ct);

        if (!response.IsValidResponse)
        {
            _logger.LogError("Search failed: {Error}", response.ElasticsearchServerError);
            return new SearchResult(
                new PagedResponse<AdSearchDocument>([], 0, query.Page, query.PageSize),
                new Dictionary<string, long>());
        }

        var categoryFacets = new Dictionary<string, long>();
        var categoriesAgg = response.Aggregations?.GetStringTerms("categories");
        if (categoriesAgg != null)
        {
            foreach (var bucket in categoriesAgg.Buckets)
                categoryFacets[bucket.Key.ToString()] = bucket.DocCount;
        }

        return new SearchResult(
            new PagedResponse<AdSearchDocument>(
                response.Documents.ToList(),
                response.Total,
                query.Page,
                query.PageSize),
            categoryFacets);
    }

    public async Task CreateIndexIfNotExistsAsync(CancellationToken ct = default)
    {
        var existsResponse = await _client.Indices.ExistsAsync(IndexName, ct);
        if (existsResponse.Exists)
            return;

        var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("russian_text", ca => ca
                            .Tokenizer("standard")
                            .Filter(new[] { "lowercase", "russian_stop", "russian_stemmer" })
                        )
                    )
                    .TokenFilters(tf => tf
                        .Stop("russian_stop", st => st.Stopwords(new[] { "_russian_" }))
                        .Stemmer("russian_stemmer", st => st.Language("russian"))
                    )
                )
            )
            .Mappings(m => m
                .Properties<AdSearchDocument>(p => p
                    .Keyword(k => k.Id)
                    .Keyword(k => k.UserId)
                    .Text(t => t.Title, td => td.Analyzer("russian_text"))
                    .Text(t => t.Description, td => td.Analyzer("russian_text"))
                    .Keyword(k => k.Category)
                    .Keyword(k => k.City)
                    .FloatNumber(n => n.Price)
                    .Keyword(k => k.Currency)
                    .Keyword(k => k.Phone)
                    .Keyword(k => k.Email!)
                    .Date(d => d.CreatedAt)
                    .Date(d => d.UpdatedAt)
                )
            ), ct);

        if (!createResponse.IsValidResponse)
            _logger.LogError("Failed to create index {Index}: {Error}", IndexName, createResponse.ElasticsearchServerError);
        else
            _logger.LogInformation("Created Elasticsearch index {Index}", IndexName);
    }

    public async Task IndexDocumentAsync(AdSearchDocument document, CancellationToken ct = default)
    {
        var response = await _client.IndexAsync(document, idx => idx
            .Index(IndexName)
            .Id(document.Id), ct);

        if (!response.IsValidResponse)
            _logger.LogError("Failed to index document {Id}: {Error}", document.Id, response.ElasticsearchServerError);
    }

    public async Task DeleteDocumentAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _client.DeleteAsync(new DeleteRequest(IndexName, id), ct);

        if (!response.IsValidResponse && response.Result != Result.NotFound)
            _logger.LogError("Failed to delete document {Id}: {Error}", id, response.ElasticsearchServerError);
    }

    public async Task RefreshIndexAsync(CancellationToken ct = default)
    {
        var response = await _client.Indices.RefreshAsync(IndexName, ct);

        if (!response.IsValidResponse)
            _logger.LogError("Failed to refresh index {Index}: {Error}", IndexName, response.ElasticsearchServerError);
    }
}