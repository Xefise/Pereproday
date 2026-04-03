using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Services;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.Catalog.Infrastructure.Services;
using Pereprodai.Shared.Application.Behaviors;
using Pereprodai.Shared.Infrastructure;

namespace Pereprodai.Catalog;

public static class CatalogModuleRegistration
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("CatalogDb"));
            options.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>());
        });

        services.AddValidatorsFromAssembly(typeof(CatalogModuleRegistration).Assembly);

        services.AddScoped<IAdRepository, AdRepository>();
        services.AddScoped<IViewCountService, ViewCountService>();

        return services;
    }
}
