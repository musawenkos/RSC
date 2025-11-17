using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadSignCapture.Core.Companies.Queries;

namespace RoadSignCapture.Core.Services
{
    public interface ICompanyService
    {
        Task AddAsync(Company company);
        Task<Company?> GetByIdAsync(int companyId);
        Task SaveChangesAsync();

        void Remove(Company company);
        bool CompanyExists(string companyName);

        Task<CompaniesDto?> GetByNameAsync(string companyName);

        Task<List<CompaniesDto>> GetAllCompaniesAsync();
    }
}
