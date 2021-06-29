using System.Threading.Tasks;

namespace RateLimitApi.Services.Abstractions
{
    public interface IRateLimitService
    {
        Task<bool> CheckLimit(string clientIp, string requestedUrl, string requestOrigin);
    }
}