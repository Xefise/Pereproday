using Microsoft.EntityFrameworkCore;
using Pereprodai.Catalog.Domain.Entities;

namespace Pereprodai.Catalog.Infrastructure;

public class CatalogDbContext : DbContext
{
    public const string Schema = "catalog";

    public DbSet<Ad> Ads => Set<Ad>();

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
