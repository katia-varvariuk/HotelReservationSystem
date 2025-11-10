using HotelReviews.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HotelReviews.WebApi.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;
    private static readonly HashSet<string> _cacheKeys = new();
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation("Cache HIT: {CacheKey}", key);
            return Task.FromResult(value);
        }

        _logger.LogInformation("Cache MISS: {CacheKey}", key);
        return Task.FromResult(default(T));
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5),
            Size = 1
        };

        _memoryCache.Set(key, value, cacheOptions);

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _cacheKeys.Add(key);
        }
        finally
        {
            _semaphore.Release();
        }

        _logger.LogInformation("Cache SET: {CacheKey}, Expiration: {Expiration}",
            key, expiration ?? TimeSpan.FromMinutes(5));
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _cacheKeys.Remove(key);

        _logger.LogInformation("Cache REMOVE: {CacheKey}", key);
        return Task.CompletedTask;
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
                _cacheKeys.Remove(key);
            }

            _logger.LogInformation("Cache REMOVE BY PREFIX: {Prefix}, Count: {Count}",
                prefix, keysToRemove.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}