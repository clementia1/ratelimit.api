using System;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using StackExchange.Redis;

namespace RateLimitApi.Services
{
    public class RedisService : IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionLazy;
        private bool _disposed;

        public RedisService(IOptions<Config> config)
        {
            var redisConfigurationOptions = ConfigurationOptions.Parse(config.Value.Redis.Host);
            _connectionLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConfigurationOptions));
        }

        public ConnectionMultiplexer Connection => _connectionLazy.Value;

        public void Dispose()
        {
            if (_disposed) return;
            Connection.Dispose();
            _disposed = true;
        }
    }
}