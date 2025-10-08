using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Companies.Commands
{
    public class CompanyHandler
    {
        private readonly ICompanyService _companyService;
        public CompanyHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<Result> CreateHandleAsync(CompanyCommands command)
        {
            var company = command.CompanyName;
            if (company == null)
                return Result.Failure("Company data is null.");

            var newCompany = new Core.Models.Company
            {
                CompanyName = command.CompanyName,
                FullAddress = command.FullAddress,
                ContactNumber = command.ContactNumber,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            await _companyService.AddAsync(newCompany);
            await _companyService.SaveChangesAsync();
            return Result.SuccessResult(newCompany);
        }

        public async Task<Result> UpdateHandleAsync(CompanyCommands command)
        {
            if (command.CompanyId == 0)
                return Result.Failure("Company data is null.");

            var existingCompany = await _companyService.GetByIdAsync(command.CompanyId);
            if (existingCompany == null)
                return Result.Failure("Company not found.");

            // Update company properties
            existingCompany.CompanyName = command.CompanyName;
            existingCompany.FullAddress = command.FullAddress;
            existingCompany.ContactNumber = command.ContactNumber;
            existingCompany.Updated = DateTime.UtcNow;


            await _companyService.SaveChangesAsync();

            return Result.SuccessResult();
        }

        public record Result(bool Success,Company? company = null, string? Error = null)
        {
            public static Result SuccessResult(Company? c = null)
            {
                return new(true,company: c);
            }

            public static Result Failure(string error) => new(false,Error: error);
        }
    }
}
