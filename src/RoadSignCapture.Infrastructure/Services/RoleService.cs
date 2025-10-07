using Microsoft.EntityFrameworkCore;
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
    public class RoleService : IRoleService
    {
        private readonly RSCDbContext _context;
        public RoleService(RSCDbContext contextRSC)
        {
            _context = contextRSC;
        }
        public async Task<Role?> GetByIdAsync(int id) => await _context.Roles.FindAsync(id);
        public async Task<List<Role>> GetAllAsync() => await _context.Roles.ToListAsync();
    }
}
