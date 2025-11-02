using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetProjectHandler>? _logger;

        public GetProjectHandler(IProjectService service, ILogger<GetProjectHandler> logger)
        {
            _projectService = service;
            _logger = logger;
        }

        public async Task<ProjectDto?> GetProjectBy(string projName,string userRole = "") 
        {
            var project = await _projectService!.GetProjectAsync(projName,userRole: userRole);
            _logger!.LogInformation("Project Query retrieved: {@project}", project);
            return project;
        
        }
    }
}
