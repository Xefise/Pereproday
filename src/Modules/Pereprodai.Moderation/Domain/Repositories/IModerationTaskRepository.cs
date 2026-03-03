using Pereprodai.Moderation.Domain.Entities;

namespace Pereprodai.Moderation.Domain.Repositories;

public interface IModerationTaskRepository
{
    Task<ModerationTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ModerationTask moderationTask, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}