using System;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Services.Abstractions;
using StackExchange.Redis;

namespace RateLimitApi.Services
{
    public class RedisConnectionService : IRedisConnectionService, IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionLazy;
        private bool _disposed;

        public RedisConnectionService(IOptions<Config> config)
        {
            var redisConfigurationOptions = ConfigurationOptions.Parse(config.Value.Redis.Host);
            _connectionLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConfigurationOptions));
        }

        public IConnectionMultiplexer Connection => _connectionLazy.Value;

        public void Dispose()
        {
            if (_disposed) return;
            Connection.Dispose();
            _disposed = true;
        }
    }
}