namespace RateLimitApi.Models
{
    public class CheckRateLimitRequest
    {
        public string RemoteIp { get; set; } = null!;
        public string RequestedUrl { get; set; } = null!;
    }
}