using RoadSignCapture.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface ICompanyService
    {
        Task AddAsync(Company company);
        Task<Company?> GetByIdAsync(int companyId);
        Task SaveChangesAsync();
    }
}
