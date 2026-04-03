using Microsoft.EntityFrameworkCore;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Infrastructure.Entities;

namespace Pereprodai.Catalog.Infrastructure;

public class CatalogDbContext : DbContext
{
    public const string Schema = "catalog";

    public DbSet<Ad> Ads => Set<Ad>();
    public DbSet<AdStatistics> AdStatistics => Set<AdStatistics>();

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
