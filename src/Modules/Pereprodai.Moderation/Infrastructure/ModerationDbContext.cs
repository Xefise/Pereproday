using Microsoft.EntityFrameworkCore;
using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Infrastructure.ReadModels;

namespace Pereprodai.Moderation.Infrastructure;

public class ModerationDbContext: DbContext
{
    public const string Schema = "moderation";

    public DbSet<ModerationTask> ModerationTasks => Set<ModerationTask>();
    public DbSet<AdReadModel> AdReadModels => Set<AdReadModel>();

    public ModerationDbContext(DbContextOptions<ModerationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModerationDbContext).Assembly);
    }
}