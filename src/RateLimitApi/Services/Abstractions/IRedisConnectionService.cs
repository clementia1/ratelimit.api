using StackExchange.Redis;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRedisConnectionService
    {
        public IConnectionMultiplexer Connection { get; }
    }
}