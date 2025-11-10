using HotelReviews.Application.Common.Interfaces;

namespace HotelReviews.WebApi.Services;

public class TwoLevelCacheService : ITwoLevelCacheService
{
    private readonly ICacheService _memoryCache; 
    private readonly IDistributedCacheService _redisCache; 
    private readonly ILogger<TwoLevelCacheService> _logger;

    public TwoLevelCacheService(
        ICacheService memoryCache,
        IDistributedCacheService redisCache,
        ILogger<TwoLevelCacheService> logger)
    {
        _memoryCache = memoryCache;
        _redisCache = redisCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var memoryValue = await _memoryCache.GetAsync<T>(key, cancellationToken);
        if (memoryValue != null)
        {
            _logger.LogInformation("L1 Cache HIT: {CacheKey}", key);
            return memoryValue;
        }

        var redisValue = await _redisCache.GetAsync<T>(key, cancellationToken);
        if (redisValue != null)
        {
            _logger.LogInformation("L2 Cache HIT, promoting to L1: {CacheKey}", key);

            await _memoryCache.SetAsync(key, redisValue, TimeSpan.FromMinutes(2), cancellationToken);

            return redisValue;
        }

        _logger.LogInformation("L1+L2 Cache MISS: {CacheKey}", key);
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? memoryExpiration = null, TimeSpan? redisExpiration = null, CancellationToken cancellationToken = default)
    {
        var memoryTtl = memoryExpiration ?? TimeSpan.FromMinutes(2);
        var redisTtl = redisExpiration ?? TimeSpan.FromMinutes(10);

        await Task.WhenAll(
            _memoryCache.SetAsync(key, value, memoryTtl, cancellationToken),
            _redisCache.SetAsync(key, value, redisTtl, cancellationToken)
        );

        _logger.LogInformation("Two-Level Cache SET: {CacheKey}, L1: {L1Ttl}, L2: {L2Ttl}", key, memoryTtl, redisTtl);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _memoryCache.RemoveAsync(key, cancellationToken),
            _redisCache.RemoveAsync(key, cancellationToken)
        );

        _logger.LogInformation("Two-Level Cache REMOVE: {CacheKey}", key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _memoryCache.RemoveByPrefixAsync(prefix, cancellationToken),
            _redisCache.RemoveByPrefixAsync(prefix, cancellationToken)
        );

        _logger.LogInformation("Two-Level Cache REMOVE BY PREFIX: {Prefix}", prefix);
    }
}