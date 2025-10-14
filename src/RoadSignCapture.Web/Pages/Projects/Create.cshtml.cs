using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Projects.Commands;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadSignCapture.Web.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly ProjectHandler _projectHandler;
        private readonly IUserService _userService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ProjectHandler projectHandler, IUserService userService, ILogger<CreateModel> logger)
        {
            _projectHandler = projectHandler;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var users = await _userService.GetAllUsersAsync();


            var userList = users.Select(u => new
            {
                u.Email,
                DisplayText = $"{u.DisplayName} : {string.Join(", ", u.Roles.Select(r => r.RoleName))} - {u.Company.CompanyName}"
            }).ToList();

            UsersList = new MultiSelectList(userList, "Email", "DisplayText");
            return Page();
        }

        [BindProperty]
        public ProjectCommand Command { get; set; } = default!;

        public MultiSelectList UsersList { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _projectHandler.CreatHandleAsync(Command);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred while creating the user.");
                // Repopulate UsersList
                var users = await _userService.GetAllUsersAsync();


                var userList = users.Select(u => new
                {
                    u.Email,
                    DisplayText = $"{u.DisplayName} : {string.Join(", ", u.Roles.Select(r => r.RoleName))} - {u.Company.CompanyName}"
                }).ToList();

                UsersList = new MultiSelectList(userList, "Email", "DisplayText");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
