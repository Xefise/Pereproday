using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Application.Behaviors;

namespace Pereprodai.Search;

public static class SearchModuleRegistration
{
    public static IServiceCollection AddSearchModule(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticClients =
            new ElasticsearchClient(new ElasticsearchClientSettings(
                new Uri(configuration["Elasticsearch:Url"]
                        ?? throw new InvalidOperationException("Elasticsearch:Url not found in configuration"))));
        services.AddSingleton(elasticClients);

        services.AddScoped<IElasticsearchService, ElasticsearchService>();

        return services;
    }
}