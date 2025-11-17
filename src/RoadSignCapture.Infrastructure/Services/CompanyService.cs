using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadSignCapture.Core.Companies.Queries;
using RoadSignCapture.Core.Users.Queries;
using Microsoft.EntityFrameworkCore;


namespace RoadSignCapture.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly RSCDbContext _context;
        public CompanyService(RSCDbContext contextRSC)
        {
            _context = contextRSC;
        }

        public async Task<Company?> GetByIdAsync(int companyId) => await _context.Companies.FindAsync(companyId);

        public async Task AddAsync(Core.Models.Company company) => await _context.Companies.AddAsync(company);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Remove(Company company)
        {
            _context.Companies.Remove(company);
        }
        public bool CompanyExists(string companyName)
        {
            return _context.Companies.Any(c => c.CompanyName == companyName);
        }

        public async Task<CompaniesDto?> GetByNameAsync(string companyName)
        {
            var company = await _context.Companies
                .Where(c => c.CompanyName == companyName)
                .Include(u => u.Users)
                .Select(c => new CompaniesDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    Users = c.Users.Select(u => new UserDto
                    {
                        Email = u.Email,
                        DisplayName = u.DisplayName,
                        CompanyName = c.CompanyName,
                        Roles = u.Roles!.Select(r => r.RoleName!).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();

            return company;
        }

        public async Task<List<CompaniesDto>> GetAllCompaniesAsync()
        {
            var companies = await _context.Companies
                .Include(u => u.Users)
                .Select(c => new CompaniesDto
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    Users = c.Users.Select(u => new UserDto
                    {
                        Email = u.Email,
                        DisplayName = u.DisplayName,
                        CompanyName = c.CompanyName,
                        Roles = u.Roles!.Select(r => r.RoleName!).ToList()
                    }).ToList()
                }).ToListAsync();

            return companies;
        }
    }
}
