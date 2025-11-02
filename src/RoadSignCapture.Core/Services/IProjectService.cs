using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Projects.Queries;
using RoadSignCapture.Core.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Services
{
    public interface IProjectService
    {
        Task<IList<Project>> GetAllProjectsAsync();
        Task<ProjectDto?> GetProjectAsync(string projectName, string userRole = "");
        Task AddAsync(Project project);
        Task SaveChangesAsync();
        void Remove(Project project);
    }
}
