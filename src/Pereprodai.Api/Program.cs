using Pereprodai.Api.Middleware;
using Pereprodai.Catalog;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.Moderation;
using Pereprodai.Moderation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Pereprodai.Api.HostedServices;
using Pereprodai.Search;
using Pereprodai.Search.Infrastructure;
using Pereprodai.Shared.Application.Behaviors;
using Pereprodai.Shared.Infrastructure.Services.Cache;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Modules
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddModerationModule(builder.Configuration);
builder.Services.AddSearchModule(builder.Configuration);

var redisConnectionString = builder.Configuration["Redis:ConnectionString"]
                            ?? throw new InvalidOperationException("Redis:ConnectionString not found in configuration");

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});


builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<ViewCountFlushService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(CatalogModuleRegistration).Assembly,
        typeof(ModerationModuleRegistration).Assembly,
        typeof(SearchModuleRegistration).Assembly
    );
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(RateLimitingBehavior<,>));
    cfg.AddOpenBehavior(typeof(CacheBehavior<,>));
});

// Health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("CatalogDb")!, name: "catalog-db")
    .AddNpgSql(builder.Configuration.GetConnectionString("ModerationDb")!, name: "moderation-db") // по приколу :)
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!)
    .AddElasticsearch(builder.Configuration["Elasticsearch:Url"]!);

// OpenAPI (Swashbuckle for .NET 8)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();

    var moderationDbContext = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
    await moderationDbContext.Database.MigrateAsync();


    var searchService = scope.ServiceProvider.GetRequiredService<IElasticsearchService>();
    await searchService.CreateIndexIfNotExistsAsync();
}

// Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<FakeAuthMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

app.Run();
