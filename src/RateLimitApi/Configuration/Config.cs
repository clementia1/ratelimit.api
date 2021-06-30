namespace RateLimitApi.Configuration
{
    public class Config
    {
        public RedisConfig Redis { get; set; } = null!;
        public IpRateLimitingConfig IpRateLimiting { get; set; } = null!;
    }
}