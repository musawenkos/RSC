using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        // user role id by email
        public async Task<int?> GetUserRoleIdByEmailAsync(string userEmail)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .Where(u => u.Email == userEmail)
                    .FirstAsync();
                if (user == null || user.Roles == null || !user.Roles.Any())
                {
                    _logger.LogWarning("User or roles not found in database: {Email}", userEmail);
                    return null; // No roles found
                }
                // Assuming a user can have multiple roles, return the first role's ID
                var roleId = user.Roles.First().RoleId;
                _logger.LogInformation("Retrieved role ID for {Email}: {RoleId}", userEmail, roleId);
                return roleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role ID for user: {Email}", userEmail);
                return null; // Fallback to null
            }
        }

        // UpdateUserRoleByEmailAsync
        public async Task<bool?> UpdateUserRoleByEmailAsync(string userEmail, int newRoleId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("User not found while updating role: {Email}", userEmail);
                    return false;
                }

                var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == newRoleId);
                if (newRole == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", newRoleId);
                    return false;
                }

                // Clear old roles and assign the new one
                user.Roles.Clear();
                user.Roles.Add(newRole);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated role for {Email} to {Role}", userEmail, newRole.RoleName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user {Email}", userEmail);
                return false;
            }
        }

    }
}
