using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadSignsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RoadSignsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("internal")]
        [AllowAnonymous]
        public IActionResult GetForInternalApp()
        {
            // Only allow if from trusted origin
            var internalToken = Request.Headers["X-Internal-App"].FirstOrDefault();
            
            if (internalToken != _configuration["ApiSettings:InternalAppToken"])
            {
                return Unauthorized();
            }
            
            return Ok(new { message = "Internal access granted" });
        }

        [HttpGet("external")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult GetForExternalClients()
        {
             var userName = User.Identity?.Name;
            var apiKey = User.FindFirst("ApiKey")?.Value;
            
            return Ok(new 
            { 
                message = "External access granted",
                authenticatedAs = userName,
                timestamp = DateTime.UtcNow
            });
        }
    }
}