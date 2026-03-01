using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Catalog.Application.Behaviors;
using Pereprodai.Catalog.Domain.Repositories;
using Pereprodai.Catalog.Infrastructure;

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

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CatalogModuleRegistration).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CatalogModuleRegistration).Assembly);

        services.AddScoped<IAdRepository, AdRepository>();

        return services;
    }
}
