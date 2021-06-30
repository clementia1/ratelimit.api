using System;
using System.Threading.Tasks;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRedisStoreService
    {
        Task<bool> Add(string key, TimeSpan? expiry = null);
        Task<string> Get(string key);
        Task<double> Increment(string key);
        Task<bool> Exists(string key);
    }
}