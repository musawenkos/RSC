using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
