using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Infrastructure.Data;

namespace RoadSignCapture.Web.Pages.Projects
{
    public class EditModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;

        public EditModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project =  await _context.Projects.FirstOrDefaultAsync(m => m.ProjectName == id);
            if (project == null)
            {
                return NotFound();
            }
            Project = project;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(Project.ProjectName))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProjectExists(string id)
        {
            return _context.Projects.Any(e => e.ProjectName == id);
        }
    }
}
