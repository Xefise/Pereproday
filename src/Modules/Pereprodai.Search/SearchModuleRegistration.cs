using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Search.Infrastructure;

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

        services.AddScoped<ElasticsearchService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SearchModuleRegistration).Assembly);
        });

        return services;
    }
}