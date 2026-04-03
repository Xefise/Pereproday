using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Infrastructure.Entities;

namespace Pereprodai.Catalog.Infrastructure.DbConfigurations;

public class AdStatisticsConfiguration : IEntityTypeConfiguration<AdStatistics>
{
    public void Configure(EntityTypeBuilder<AdStatistics> builder)
    {
        builder.ToTable("adStatistics", CatalogDbContext.Schema)
            .HasKey(a => a.AdId);

        builder.Property(x => x.Views).HasDefaultValue(0);

        builder.HasOne<Ad>()  // без навигационного свойства
            .WithOne()
            .HasForeignKey<AdStatistics>(x => x.AdId);
    }
}