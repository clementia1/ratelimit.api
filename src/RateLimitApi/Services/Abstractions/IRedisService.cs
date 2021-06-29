using StackExchange.Redis;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRedisService
    {
        ConnectionMultiplexer Connection { get; }
    }
}