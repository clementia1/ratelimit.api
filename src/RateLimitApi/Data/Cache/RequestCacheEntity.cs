namespace RateLimitApi.Data.Cache
{
    public class RequestCacheEntity
    {
        public string? ClientIp { get; set; }
        public string? RequestedUrl { get; set; }
        public string? RequestOrigin { get; set; }
    }
}