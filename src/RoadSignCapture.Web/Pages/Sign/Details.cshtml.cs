using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Infrastructure.Data;

namespace RoadSignCapture.Web.Pages.Sign
{
    public class DetailsModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;

        public DetailsModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context)
        {
            _context = context;
        }

        public RoadSignCapture.Core.Models.Sign Sign { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sign = await _context.Signs.FirstOrDefaultAsync(m => m.Id == id);

            if (sign is not null)
            {
                Sign = sign;

                return Page();
            }

            return NotFound();
        }
    }
}
