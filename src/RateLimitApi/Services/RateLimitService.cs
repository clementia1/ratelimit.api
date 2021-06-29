using System.Threading.Tasks;
using RateLimitApi.Data.Cache;
using RateLimitApi.Services.Abstractions;

namespace RateLimitApi.Services
{
    public class RateLimitService
    {
        private readonly ICacheService<PizzaCacheEntity> _cacheService;

        public RateLimitService(ICacheService<PizzaCacheEntity> cacheService)
        {
            _cacheService = cacheService;
        }
        
        public async Task<bool> CheckLimit(string clientIp, string requestedUrl, string requestOrigin)
        {
            var userKey = GetUserKey(clientIp, requestedUrl, requestOrigin);
            var userRateLimit = _cacheService.GetAsync(userKey);
            await _cacheService.AddOrUpdateAsync(new ClientCacheEntity()
            {
                ClientIp = clientIp,
                RequestedUrl = requestedUrl,
                RequestOrigin = requestOrigin
            }, "userName");
        }

        public async Task AddItem(string clientIp, string requestedUrl, string requestOrigin)
        {
            
        }

        private string GetUserKey(string clientIp, string requestedUrl, string requestOrigin)
        {
            return $"{requestOrigin}_{clientIp}_{requestedUrl}";
        }
    }
}