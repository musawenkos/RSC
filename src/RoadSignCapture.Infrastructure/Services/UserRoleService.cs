using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{
    public class UserRoleService : IUserRole
    {
        private readonly RSCDbContext _context;
        private readonly ILogger<UserRoleService> _logger;
        public UserRoleService(RSCDbContext contextRSC, ILogger<UserRoleService> logger)
        {
            _context = contextRSC;
            _logger = logger;
        }
        public async Task<string[]> GetUserRolesAsync(string userEmail)
        {
            try
            {
                // Example: Retrieve roles from your database
                // Adjust this query based on your actual database schema
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .Where(u => u.Email == userEmail)
                    .FirstAsync();

                if (user == null)
                {
                    _logger.LogWarning("User not found in database: {Email}", userEmail);
                    return new[] { "Viewer" }; // Default role
                }

                var roles = user.Roles?.Select(ur => ur.RoleName).ToArray() ?? new[] { "Viewer" };

                _logger.LogInformation("Retrieved roles for {Email}: {Roles}", userEmail, string.Join(", ", roles));

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user: {Email}", userEmail);
                return new[] { "Viewer" }; // Fallback to default role
            }
        }
    }
}
