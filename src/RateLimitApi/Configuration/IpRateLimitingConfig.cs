using System;

namespace RateLimitApi.Configuration
{
    public class IpRateLimitingConfig
    {
        public TimeSpan Period { get; set; }
        public int Limit { get; set; }
    }
}