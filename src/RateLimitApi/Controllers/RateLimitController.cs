using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitApi.Configuration;

namespace RateLimitController.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class RateLimitController : ControllerBase
    {
        private readonly ILogger<RateLimitController> _logger;
        private readonly Config _config;

        public RateLimitController(
            ILogger<RateLimitController> logger,
            IOptions<Config> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> CheckLimit(string userKey)
        {
        }
    }
}