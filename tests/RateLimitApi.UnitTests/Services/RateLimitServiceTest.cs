using System;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Services;
using RateLimitApi.Services.Abstractions;
using Xunit;

namespace RateLimitApi.UnitTests.Services
{
    public class RateLimitServiceTest
    {
        private readonly IRateLimitService _rateLimitService;
        private readonly Mock<IRedisStoreService> _redisStoreService;
        private IOptions<Config> _config;
        
        public RateLimitServiceTest()
        {
            var customConfig = new Config(){ IpRateLimiting = new IpRateLimitingConfig { Limit = 3 }};
            _config = Options.Create(customConfig);
            _redisStoreService = new Mock<IRedisStoreService>();

            _redisStoreService.Setup(expression: x => x.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            _redisStoreService.Setup(expression: x => x.Add(It.IsAny<string>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync(true);
            _redisStoreService.Setup(expression: x => 
                    x.Increment(It.Is<string>(str => str == "PizzaApi_127.0.0.1_/api/v1/GetById?id=3")))
                .ReturnsAsync(1);
            _redisStoreService.Setup(expression: x => 
                    x.Increment(It.Is<string>(str => str == "PizzaApi_127.0.0.1_/api/v1/GetById?id=2")))
                .ReturnsAsync(4);

            _rateLimitService = new RateLimitService(_redisStoreService.Object, _config);
        }

        [Fact]
        public async Task CheckLimit_Success()
        {
            //arrange
            var reqOrigin = "PizzaApi";
            var remoteIp = "127.0.0.1";
            var requestedUrl = "/api/v1/GetById?id=3";
            
            //act
            var result = await _rateLimitService.CheckLimit(remoteIp, requestedUrl, reqOrigin);

            //assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task CheckLimit_Failure()
        {
            //arrange
            var reqOrigin = "PizzaApi";
            var remoteIp = "127.0.0.1";
            var requestedUrl = "/api/v1/GetById?id=2";
            
            //act
            var result = await _rateLimitService.CheckLimit(remoteIp, requestedUrl, reqOrigin);

            //assert
            result.Should().BeFalse();
        }
    }
}