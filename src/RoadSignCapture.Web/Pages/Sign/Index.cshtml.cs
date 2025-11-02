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
    public class IndexModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;

        public IndexModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context)
        {
            _context = context;
        }

        public IList<RoadSignCapture.Core.Models.Sign> Sign { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Sign = await _context.Signs
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser)
                .Include(s => s.Project).ToListAsync();
        }
    }
}
