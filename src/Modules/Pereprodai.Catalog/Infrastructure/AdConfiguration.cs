using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Domain.Enums;

namespace Pereprodai.Catalog.Infrastructure;

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.ToTable("ads", CatalogDbContext.Schema)
            .HasKey(a => a.Id);

        builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(4000).IsRequired();
        builder.Property(a => a.Category).HasMaxLength(100).IsRequired();
        builder.OwnsOne(a => a.Price, price =>
        {
            price.Property(p => p.Amount);
            price.Property(p => p.Currency);
        });
        builder.OwnsOne(a => a.Location, location =>
        {
            location.Property(l => l.City).HasMaxLength(100);
        });
        builder.OwnsOne(a => a.ContactInfo, contact =>
        {
            contact.Property(c => c.Phone).HasMaxLength(14);
            contact.Property(c => c.Email).HasMaxLength(100);
        });

        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Category);
    }
}
