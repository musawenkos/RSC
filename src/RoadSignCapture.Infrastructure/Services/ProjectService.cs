using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Projects.Queries;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Infrastructure.Services
{

    public class ProjectService : IProjectService
    {
        private readonly RSCDbContext _context;
        public ProjectService(RSCDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(u => u.Users)
                .OrderBy(p => p.Created)
                .ToListAsync();
        }

        public async Task<ProjectDto?> GetProjectAsync(string projectName)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.ProjectName == projectName)
                .Include(u => u.Users)
                .FirstAsync();

            if (project == null) return null;

            return new ProjectDto
            { 
                ProjectName = projectName,
                ProjectDescription = project.Description,
                Created = project.Created,
                Updated = project.Updated,
                Clients = [.. project.Users]
            };
 
        }
        public async Task AddAsync(Project project) => await _context.Projects.AddAsync(project);
        public void Remove(Project project) => _context.Projects.Remove(project);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
