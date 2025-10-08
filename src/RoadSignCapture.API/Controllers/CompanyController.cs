using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Companies.Commands;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyHandler? _companyHandler;

        public CompanyController(CompanyHandler companyHandler)
        {
            _companyHandler = companyHandler;
        }

        // POST: api/company
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCommands command)
        {
            var result = await _companyHandler!.CreateHandleAsync(command);
            return result.Success ? Ok(result) : BadRequest(result.Error);
        }
    }
}
