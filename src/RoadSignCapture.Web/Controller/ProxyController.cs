using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Web.Services;

namespace RoadSignCapture.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IApiClientService _apiClientService;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(IApiClientService apiClientService, ILogger<ProxyController> logger)
        {
            _apiClientService = apiClientService;
            _logger = logger;
        }

        [HttpPost("create-company")]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CompanyCommands command)
        {
            try
            {
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(command),
                    System.Text.Encoding.UTF8,
                    "application/json");
                var response = await _apiClientService.PostAsync("api/company", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return Ok(System.Text.Json.JsonSerializer.Deserialize<object>(responseData));
                }
                else
                {
                    _logger.LogWarning("API request failed with status code: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { error = "API request failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the request.");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}