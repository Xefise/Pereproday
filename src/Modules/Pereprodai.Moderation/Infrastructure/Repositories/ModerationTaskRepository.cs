using Microsoft.EntityFrameworkCore;
using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Domain.Repositories;

namespace Pereprodai.Moderation.Infrastructure.Repositories;

public class ModerationTaskRepository : IModerationTaskRepository
{
    private readonly ModerationDbContext _dbContext;

    public ModerationTaskRepository(ModerationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ModerationTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ModerationTasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(ModerationTask moderationTask, CancellationToken cancellationToken = default)
    {
        await _dbContext.ModerationTasks.AddAsync(moderationTask, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}