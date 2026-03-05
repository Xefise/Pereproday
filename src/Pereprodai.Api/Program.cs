using Pereprodai.Api.Middleware;
using Pereprodai.Catalog;
using Pereprodai.Catalog.Infrastructure;
using Pereprodai.Moderation;
using Pereprodai.Moderation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Modules
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddModerationModule(builder.Configuration);

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
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

// Auto-apply migrations in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();

    var moderationDbContext = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
    await moderationDbContext.Database.MigrateAsync();
}

// Middleware pipeline
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
