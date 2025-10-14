using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;

namespace RoadSignCapture.Web.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly IProjectService _projectService;

        public IndexModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public IList<Project> Project { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Project = await _projectService.GetAllProjectsAsync();
        }
    }
}
