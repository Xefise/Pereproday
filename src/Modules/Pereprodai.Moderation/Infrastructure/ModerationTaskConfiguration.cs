using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Infrastructure.ReadModels;

namespace Pereprodai.Moderation.Infrastructure;

public class ModerationTaskConfiguration : IEntityTypeConfiguration<ModerationTask>
{
    public void Configure(EntityTypeBuilder<ModerationTask> builder)
    {
        builder.ToTable("moderation_tasks", ModerationDbContext.Schema)
            .HasKey(t => t.Id);

        builder.Property(x => x.RejectionReason).HasMaxLength(2000);


        builder.HasIndex(x => x.AdId);
        builder.HasIndex(x => new { x.Status, x.CreatedAt });


        builder.HasOne<AdReadModel>()
            .WithMany(x => x.ModerationTasks)
            .HasForeignKey(x => x.AdId);
    }
}