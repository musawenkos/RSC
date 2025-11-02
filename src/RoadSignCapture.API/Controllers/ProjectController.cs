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

        public ProjectController(GetProjectHandler? projecthandler)
        {
            _getProjecthandler = projecthandler;
        }

        //api/project/{projectName}
        [HttpGet("{projectName}")]
        public async Task<IActionResult> GetProjectClientBy(string projectName)
        {
            var project = await _getProjecthandler!.GetProjectBy(projectName, "Client");
            return project == null ? NotFound(project) : Ok(project);
        }
    }
}
