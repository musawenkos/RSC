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

namespace RoadSignCapture.Web.Pages.Sign
{
    public class EditModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;

        public EditModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RoadSignCapture.Core.Models.Sign Sign { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sign =  await _context.Signs.FirstOrDefaultAsync(m => m.Id == id);
            if (sign == null)
            {
                return NotFound();
            }
            Sign = sign;
           ViewData["ClientId"] = new SelectList(_context.Users, "Email", "Email");
           ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Email", "Email");
           ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectName", "ProjectName");
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

            _context.Attach(Sign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SignExists(Sign.Id))
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

        private bool SignExists(Guid id)
        {
            return _context.Signs.Any(e => e.Id == id);
        }
    }
}
