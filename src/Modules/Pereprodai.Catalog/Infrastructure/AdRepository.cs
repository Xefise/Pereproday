using Microsoft.EntityFrameworkCore;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.Repositories;

namespace Pereprodai.Catalog.Infrastructure;

public class AdRepository : IAdRepository
{
    private readonly CatalogDbContext _dbContext;

    public AdRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Ad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Ad ad, CancellationToken cancellationToken = default)
    {
        await _dbContext.Ads.AddAsync(ad, cancellationToken);
    }

    public async Task<(List<Ad> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        AdStatus? statusFilter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Ads.Where(x => x.UserId == userId);
        if(statusFilter.HasValue) query = query.Where(x => x.Status == statusFilter.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
