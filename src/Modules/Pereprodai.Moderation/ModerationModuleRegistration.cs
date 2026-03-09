using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pereprodai.Moderation.Domain.Repositories;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Moderation.Infrastructure.Repositories;
using Pereprodai.Shared.Application.Behaviors;
using Pereprodai.Shared.Infrastructure;

namespace Pereprodai.Moderation;

public static class ModerationModuleRegistration
{
    public static IServiceCollection AddModerationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventDispatchInterceptor>();

        services.AddDbContext<ModerationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ModerationDb"));
            options.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>());
        });

        services.AddValidatorsFromAssembly(typeof(ModerationModuleRegistration).Assembly);

        services.AddScoped<IModerationTaskRepository, ModerationTaskRepository>();

        return services;
    }
}