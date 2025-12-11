using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Projects.Queries;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly GetProjectHandler? _getProjecthandler;
        private readonly ILogger<ProjectController>? _logger;

        public ProjectController(GetProjectHandler? projecthandler, ILogger<ProjectController>? logger)
        {
            _getProjecthandler = projecthandler;
            _logger = logger;
        }

        //api/project/{projectName}
        [Authorize]
        [HttpGet("{projectName}")]
        public async Task<IActionResult> GetProjectClientBy(string projectName)
        {
            var project = await _getProjecthandler!.GetProjectBy(projectName, "Client");
            _logger!.LogInformation("Project retrieved: {@project}", project);
            return project == null ? NotFound(project) : Ok(project);
        }
    }
}
