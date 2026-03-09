using System.Text.Json;
using MediatR;
using Pereprodai.Shared.Infrastructure.Services.Cache;

namespace Pereprodai.Shared.Application.Behaviors;

public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICacheService _cacheService;

    public CacheBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if(request is not ICacheable cacheable) return await next();
        var key = $"{cacheable.CachePrefix}:{JsonSerializer.Serialize(request)}";
        var data = await _cacheService.GetAsync<TResponse>(key, ct);
        if (data != null) return data;
        data = await next();
        await _cacheService.SetAsync(key, data, cacheable.CacheDuration, ct);
        return data;
    }
}