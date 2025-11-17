using RoadSignCapture.Core.Services;
using Microsoft.Extensions.Logging;

namespace RoadSignCapture.Core.Companies.Queries
{
    public class GetCompaniesHandler
    {
        private readonly ICompanyService? _companyService;
        private readonly ILogger<GetCompaniesHandler>? _logger;

        public GetCompaniesHandler(ICompanyService service, ILogger<GetCompaniesHandler> logger)
        {
            _companyService = service;
            _logger = logger;
        }

        public async Task<CompaniesDto> GetCompanyByNameAsync(string companyName)
        {
            var company = await  _companyService!.GetByNameAsync(companyName);
            _logger!.LogInformation("Companies Query retrieved: {@companies}", company);
            return company!;
        }

        public async Task<List<CompaniesDto>> GetAllCCompaniesAsync()
        {
            var allCompanies = await _companyService!.GetAllCompaniesAsync(); 
            _logger!.LogInformation("All Companies Query retrieved: {@companies}", allCompanies);
            return allCompanies;
        }
    }
}