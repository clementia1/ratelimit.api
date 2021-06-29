using System;

namespace RateLimitApi.Data.Cache
{
    public class PizzaCacheEntity : ICacheEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public double Weight { get; set; }
    }
}