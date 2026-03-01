using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Domain.Enums;

namespace Pereprodai.Catalog.Domain.Repositories;

public interface IAdRepository
{
    Task<Ad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Ad ad, CancellationToken cancellationToken = default);
    Task<(List<Ad> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        AdStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
