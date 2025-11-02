using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Projects.Queries
{
    public class GetProjectHandler
    {
        private readonly IProjectService? _projectService;

        public GetProjectHandler(IProjectService service)
        {
            _projectService = service;
        }

        public async Task<ProjectDto?> GetProjectBy(string projName,string userRole = "") 
        {
            return await _projectService!.GetProjectAsync(projName,userRole: userRole);
        
        }
    }
}
