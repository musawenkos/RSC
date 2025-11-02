using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Projects.Queries;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Queries;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{

    public class ProjectService : IProjectService
    {
        private readonly RSCDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<ProjectService> _logger;
        public ProjectService(RSCDbContext context, IUserService userService, ILogger<ProjectService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IList<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(u => u.Users)
                .OrderBy(p => p.Created)
                .ToListAsync();
        }

        public async Task<ProjectDto?> GetProjectAsync(string projectName, string userRole = "")
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.ProjectName == projectName)
                .Include(u => u.Users)
                .FirstAsync();

            if (project == null) return null;

            if (userRole != "")
            {
                var users = project.Users.ToList();
                _logger.LogInformation($"Users : {users}");

                var user = await _userService.GetClientFrom(users);

                _logger.LogInformation($"User : {user}");

                return new ProjectDto
                {
                    Clients = [user]
                };
            }

            var clients = new List<UserDto>();
            foreach (var user in project.Users)
            {
                var u = new UserDto
                {
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    CompanyName = user.Company.CompanyName,
                    Roles = user.Roles.Select(r => r.RoleName).ToList()
                };
                clients.Add(u);
            }

            _logger!.LogInformation("Project Function completed: {projectName}", projectName);

            return new ProjectDto
            { 
                ProjectName = projectName,
                ProjectDescription = project.Description,
                Created = project.Created,
                Updated = project.Updated,
                Clients = [.. clients]
            };
 
        }
        public async Task AddAsync(Project project) => await _context.Projects.AddAsync(project);
        public void Remove(Project project) => _context.Projects.Remove(project);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
