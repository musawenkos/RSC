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
    public class UserService : IUserService
    {
        private readonly RSCDbContext _context;
        public UserService(RSCDbContext contextRSC)
        {
            _context = contextRSC;
        }
        public async Task<IList<User>> GetAllUsersAsync()
        {
            //Exclude authenticated user

            return await _context.Users
                .AsNoTracking()
                .Include(c => c.Company)
                .Include(r => r.Roles)
                .OrderByDescending(u => u.Created)
                .ToListAsync();
        }
    }
}
