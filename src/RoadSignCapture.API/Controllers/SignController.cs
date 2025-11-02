using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Signs.Queries;
using RoadSignCapture.Infrastructure.Services;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private readonly GetSignHandler? _signHandler;
        private readonly ILogger<SignController> _logger;
        private readonly ICacheService _cache;

        public SignController(GetSignHandler? signHandler, ICacheService cache, ILogger<SignController> logger)
        {
            _signHandler = signHandler;
            _cache = cache;
            _logger = logger;
        }

        //api/sign/{projecName}
        [HttpGet("{projectName}")]

        public async Task<IActionResult> GetSignsProjectBy(string projectName)
        {
            var cacheKey = $"signs:{projectName}";
            var cachedSigns = await _cache.GetAsync<List<SignDto>>(cacheKey);

            if (cachedSigns is not null)
            {
                _logger.LogInformation("Cache hit for {projectName}", projectName);
                return Ok(cachedSigns);

            }


            var signs = await _signHandler!.GetListSignBy(projectName);

            if (signs.Count == 0)
            {
                var message = $"No signs are available for the project: {signs}";
                return NotFound(message);
            }
                

            await _cache.SetAsync(cacheKey, signs, TimeSpan.FromMinutes(20));
            _logger.LogInformation("Cache miss, storing data for {projectName}", projectName);

            return Ok(signs);

        }
    }
}
