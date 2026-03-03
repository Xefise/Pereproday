using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pereprodai.Moderation.Infrastructure.ReadModels;

namespace Pereprodai.Moderation.Infrastructure;

public class AdReadModelConfiguration : IEntityTypeConfiguration<AdReadModel>
{
    public void Configure(EntityTypeBuilder<AdReadModel> builder)
    {
        builder.ToTable("ads_read_model", ModerationDbContext.Schema).HasKey(x => x.AdId);
    }
}