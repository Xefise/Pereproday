using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.Extensions.Logging;
using Pereprodai.Search.Documents;

namespace Pereprodai.Search.Infrastructure;

public class ElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string IndexName = "ads";

    public ElasticsearchService(ElasticsearchClient client, ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _logger = logger;
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
        var response = await _client.DeleteAsync(IndexName, id, ct);

        if (!response.IsValidResponse && response.Result != Result.NotFound)
            _logger.LogError("Failed to delete document {Id}: {Error}", id, response.ElasticsearchServerError);
    }
}