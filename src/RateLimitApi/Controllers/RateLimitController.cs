using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;
using RateLimitApi.Models;
using RateLimitApi.Services;
using RateLimitApi.Services.Abstractions;

namespace RateLimitController.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class RateLimitController : ControllerBase
    {
        private readonly IRateLimitService _rateLimitService;
        private readonly ILogger<RateLimitController> _logger;
        private readonly Config _config;

        public RateLimitController(
            ILogger<RateLimitController> logger,
            IOptions<Config> config,
            IRateLimitService rateLimitService)
        {
            _logger = logger;
            _config = config.Value;
            _rateLimitService = rateLimitService;
        }

        [HttpPost]
        public async Task<IActionResult> Check(CheckRateLimitRequest request)
        {
            Request.Headers.TryGetValue("Origin", out var requestOrigin);
            var result = await _rateLimitService.CheckLimit(request.RemoteIp, request.RequestedUrl, requestOrigin);
            return result ? Ok() : StatusCode(429);
        }
    }
}