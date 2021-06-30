using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Services.Abstractions;
using StackExchange.Redis;

namespace RateLimitApi.Services
{
    public class RedisStoreService : IRedisStoreService
    {
        private readonly ILogger<RedisStoreService> _logger;
        private readonly IRedisConnectionService _redisCacheConnectionService;
        private readonly Config _config;

        public RedisStoreService(
            ILogger<RedisStoreService> logger,
            IRedisConnectionService redisCacheConnectionService,
            IOptions<Config> config)
        {
            _logger = logger;
            _redisCacheConnectionService = redisCacheConnectionService;
            _config = config.Value;
        }

        public async Task<bool> Add(string key, TimeSpan? expiry = null)
        {
            var redis = GetRedisDatabase();
            expiry ??= _config.IpRateLimiting.Period;
            return await redis.StringSetAsync(key, 1, expiry);
        }

        public async Task<string> Get(string key)
        {
            var redis = GetRedisDatabase();
            return await redis.StringGetAsync(key);
        }

        public async Task<double> Increment(string key)
        {
            var redis = GetRedisDatabase();
            return await redis.StringIncrementAsync(key);
        }

        public async Task<bool> Exists(string key)
        {
            var redis = GetRedisDatabase();
            return await redis.KeyExistsAsync(key);
        }
        
        private IDatabase GetRedisDatabase() => _redisCacheConnectionService.Connection.GetDatabase();
    }
}