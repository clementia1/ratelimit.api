using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Data.Cache;
using RateLimitApi.Services.Abstractions;

namespace RateLimitApi.Services
{
    public class CacheService<TCacheEntity> : ICacheService<TCacheEntity>
        where TCacheEntity : class, ICacheEntity
    {
        private readonly ILogger<CacheService<TCacheEntity>> _logger;
        private readonly IRedisConnectionService _redisCacheConnectionService;
        private readonly Config _config;

        public CacheService(
            ILogger<CacheService<TCacheEntity>> logger,
            IRedisConnectionService redisCacheConnectionService,
            IOptions<Config> config)
        {
            _logger = logger;
            _redisCacheConnectionService = redisCacheConnectionService;
            _config = config.Value;
        }

        public Task AddOrUpdateAsync(TCacheEntity entity, string userName) => AddOrUpdateInternalAsync(entity, userName);

        public async Task<TCacheEntity?> GetAsync(string itemKey)
        {
            var redis = GetRedisDatabase();
            var serialized = await redis.StringGetAsync(itemKey);

            return !string.IsNullOrEmpty(serialized) ? JsonSerializer.Deserialize<TCacheEntity>(serialized) : null;
        }

        public async Task RemoveAsync(string clientIp, string requestedUrl, string requestOrigin)
        {
            var redis = GetRedisDatabase();

            var cacheKey = GetItemCacheKey(clientIp, requestedUrl, requestOrigin);

            if (await redis.KeyDeleteAsync(cacheKey))
            {
                _logger.LogInformation($"{typeof(TCacheEntity).Name} with key {cacheKey} was removed from cache");
            }
            else
            {
                _logger.LogWarning($"Can't remove {typeof(TCacheEntity).Namespace} with key {cacheKey}");
            }
        }

        private string GetItemCacheKey(string clientIp, string requestedUrl, string requestOrigin) => $"{requestOrigin}_{clientIp}_{requestedUrl}";

        private async Task AddOrUpdateInternalAsync(TCacheEntity entity, string userName, IDatabase? redis = null, TimeSpan? expiry = null)
        {
            redis ??= GetRedisDatabase();
            expiry ??= _config.Redis.CacheTimeout;

            var cacheKey = GetItemCacheKey(entity.Id, userName);
            var serialized = JsonSerializer.Serialize(entity);

            if (await redis.StringSetAsync(cacheKey, serialized, expiry))
            {
                _logger.LogInformation($"{typeof(TCacheEntity).Name} for user {userName} cached. New data: {serialized}");
            }
            else
            {
                _logger.LogInformation($"{typeof(TCacheEntity).Name} for user {userName} updated. New data: {serialized}");
            }
        }

        private IDatabase GetRedisDatabase() => _redisCacheConnectionService.Connection.GetDatabase();
    }
}