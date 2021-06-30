using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Services.Abstractions;

namespace RateLimitApi.Services
{
    public class RateLimitService
    {
        private readonly IRedisStoreService _redisStoreService;
        private readonly Config _config;

        public RateLimitService(
            IRedisStoreService redisStoreService,
            IOptions<Config> config)
        {
            _redisStoreService = redisStoreService;
            _config = config.Value;
        }
        
        public async Task<bool> CheckLimit(string clientIp, string requestedUrl, string requestOrigin)
        {
            var clientKey = GetClientKey(clientIp, requestedUrl, requestOrigin);
            var keyExists = await _redisStoreService.Exists(clientKey);

            if (keyExists)
            {
                var keyValue = await _redisStoreService.Increment(clientKey);
                return !(keyValue > _config.IpRateLimiting.Limit);
            }
            else
            {
                await _redisStoreService.Add(clientKey);
                return true;
            }
        }

        private string GetClientKey(string clientIp, string requestedUrl, string requestOrigin)
        {
            return $"{requestOrigin}_{clientIp}_{requestedUrl}";
        }
    }
}