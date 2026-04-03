using Microsoft.EntityFrameworkCore;
using Pereprodai.Catalog.Application.Services;
using Pereprodai.Catalog.Infrastructure.Entities;
using StackExchange.Redis;

namespace Pereprodai.Catalog.Infrastructure.Services;

public class ViewCountService: IViewCountService
{
    private readonly IDatabase _redis;
    private readonly CatalogDbContext _context;

    public ViewCountService(IConnectionMultiplexer connection, CatalogDbContext context)
    {
        _redis = connection.GetDatabase();
        _context = context;
    }

    public async Task<Dictionary<Guid, long>> GetViewCountsAsync(IEnumerable<Guid> adIds)
    {
        return await _context.AdStatistics
            .Where(x => adIds.Contains(x.AdId))
            .ToDictionaryAsync(x => x.AdId, x => x.Views);
    }

    public async Task<long> GetViewCountAsync(Guid adId)
    {
        return await _context.AdStatistics
            .Where(x => x.AdId == adId)
            .Select(x => x.Views)
            .FirstOrDefaultAsync();
    }

    public async Task IncrementViewCountAsync(Guid adId)
    {
        await _redis.StringIncrementAsync($"ad:views:{adId}");
        await _redis.SetAddAsync($"ad:views:dirty", adId.ToString());
    }

    public async Task FlushViewCountsAsync()
    {
        var list = await _redis.SetMembersAsync("ad:views:dirty");

        foreach (var id in list)
        {
            var redisValue = await _redis.StringGetDeleteAsync("ad:views:" + id);
            await _redis.SetRemoveAsync("ad:views:dirty", id);
            if (redisValue.IsNullOrEmpty) continue;
            var value = long.Parse(redisValue.ToString());
            var adId = Guid.Parse(id.ToString());
            var stats = await _context.AdStatistics.FirstOrDefaultAsync(x => x.AdId == adId);

            if(stats != null) stats.Views += value;
            else
            {
                stats = new AdStatistics()
                {
                    AdId = adId,
                    Views = value,
                };
                await _context.AdStatistics.AddAsync(stats);
            }
        }

        await _context.SaveChangesAsync();
    }
}