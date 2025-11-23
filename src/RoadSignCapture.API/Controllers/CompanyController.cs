using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Core.Companies.Queries;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyHandler? _companyHandler;
        private readonly GetCompaniesHandler? _getCompaniesHandler;
        private readonly FluentValidation.IValidator<CompanyCommands> _companyValidator;

        public CompanyController(CompanyHandler companyHandler, GetCompaniesHandler getCompaniesHandler, FluentValidation.IValidator<CompanyCommands> companyValidator)
        {
            _companyHandler = companyHandler;
            _getCompaniesHandler = getCompaniesHandler;
            _companyValidator = companyValidator;
        }

        // POST: api/company
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCommands command)
        {
            var validationResult = _companyValidator.Validate(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var problemDetails = new HttpValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                    Detail = "See the errors property for details.",
                    Instance = HttpContext.Request.Path
                };

                return BadRequest(problemDetails);
            }
            var result = await _companyHandler!.CreateHandleAsync(command);
            return result.Success ? Ok(result) : BadRequest(result.Error);
        }

        //POST: api/company/delete/{companyId}
        [HttpPost("delete/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await _companyHandler!.DeleteHandleAsync(companyId);
            return result.Success ? Ok(result) : BadRequest(result.Error);
        }
        

        //GET api/company/{companyName}
        [HttpGet("{companyName}")]
        public async Task<IActionResult> GetCompanyByName(string companyName)
        {
            var company = await _getCompaniesHandler!.GetCompanyByNameAsync(companyName);
            return company == null ? NotFound() : Ok(company);
        }

        //GET api/company/ 
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies = await _getCompaniesHandler!.GetAllCCompaniesAsync();
            return companies == null ? NotFound() : Ok(companies);
        }
    }
}
