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
        public async Task<IList<User>> GetAllUsersAsync(string authenticatedUser)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Email != authenticatedUser)
                .Include(c => c.Company)
                .Include(r => r.Roles)
                .OrderByDescending(u => u.Created)
                .ToListAsync();
        }

        public async Task<User?> GetUserBy(string userEmail) => await _context.Users.FindAsync(userEmail);
        public bool UserExists(string email) => _context.Users.Any(e => e.Email == email);
        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
        public void Remove(User user) => _context.Users.Remove(user);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        
    }
}
