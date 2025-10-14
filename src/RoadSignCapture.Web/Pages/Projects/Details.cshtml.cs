using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Infrastructure.Data;

namespace RoadSignCapture.Web.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;

        public DetailsModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context)
        {
            _context = context;
        }

        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectName == id);

            if (project is not null)
            {
                Project = project;

                return Page();
            }

            return NotFound();
        }
    }
}
