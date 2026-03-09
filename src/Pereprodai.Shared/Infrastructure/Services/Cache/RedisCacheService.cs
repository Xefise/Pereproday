using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Pereprodai.Shared.Infrastructure.Services.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }


    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var json = await _cache.GetStringAsync(key, ct);
        if (json == null) return default;
        var obj = JsonSerializer.Deserialize<T>(json);
        return obj;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(
            key, json,
            new DistributedCacheEntryOptions{ AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(15) },
            ct);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var server = _redis.GetServers().First();
        var asyncKeys = server.KeysAsync(pattern: $"{prefix}*");
        var db = _redis.GetDatabase();
        var keys = new List<RedisKey>();
        await foreach (var key in asyncKeys)
        {
            keys.Add(key);
        }

        await db.KeyDeleteAsync(keys.ToArray());
    }
}