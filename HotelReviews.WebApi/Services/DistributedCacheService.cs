using HotelReviews.Application.Common.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace HotelReviews.WebApi.Services;

public class DistributedCacheService : IDistributedCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly IDatabase _database;

    public DistributedCacheService(
        IConnectionMultiplexer redis,
        ILogger<DistributedCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
        _database = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                _logger.LogInformation("Redis Cache MISS: {CacheKey}", key);
                return default;
            }

            _logger.LogInformation("Redis Cache HIT: {CacheKey}", key);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis GET error for key: {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            var ttl = expiration ?? TimeSpan.FromMinutes(10);

            await _database.StringSetAsync(key, serialized, ttl);

            _logger.LogInformation("Redis Cache SET: {CacheKey}, Expiration: {Expiration}", key, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis SET error for key: {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogInformation("Redis Cache REMOVE: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis REMOVE error for key: {CacheKey}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                _logger.LogInformation("Redis Cache REMOVE BY PREFIX: {Prefix}, Count: {Count}", prefix, keys.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis REMOVE BY PREFIX error for: {Prefix}", prefix);
        }
    }
}