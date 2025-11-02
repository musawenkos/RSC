using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Signs.Commands;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadSignCapture.Web.Pages.Sign
{
    public class CreateModel : PageModel
    {
        private readonly RoadSignCapture.Infrastructure.Data.RSCDbContext _context;
        private readonly SignHandler _signHandler;
        private readonly ILogger<CreateModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(RoadSignCapture.Infrastructure.Data.RSCDbContext context, SignHandler signHandler, ILogger<CreateModel> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _signHandler = signHandler;
            _logger = logger;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectName", "ProjectName");
            return Page();
        }

        [BindProperty]
        public SignCommand Command { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            var result = await _signHandler.CreateHandlerAsync(Command, _environment.WebRootPath);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred while creating the sign.");
                ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectName", "ProjectName");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
