using MediatR;
using Microsoft.Extensions.Logging;
using Pereprodai.Shared.Application.Exceptions;
using StackExchange.Redis;

namespace Pereprodai.Shared.Application.Behaviors;

public class RateLimitingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<RateLimitingBehavior<TRequest, TResponse>> _logger;
    private readonly IDatabase _redis;

    public RateLimitingBehavior(IConnectionMultiplexer connection, ILogger<RateLimitingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _redis = connection.GetDatabase();
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not IRateLimited limited) return await next();

        var key = $"rl:{limited.RateLimitKey}";
        long count;
        try
        {
            count = await _redis.StringIncrementAsync(key);
            await _redis.KeyExpireAsync(key, limited.Window, ExpireWhen.HasNoExpiry);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Rate limiter недоступен");
            return await next();
        }


        if (count > limited.Limit)
            throw new RateLimitExceededException(limited.Limit, limited.Window);


        return await next();
    }
}