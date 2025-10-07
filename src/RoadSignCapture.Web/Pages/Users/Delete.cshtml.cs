using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadSignCapture.Web.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly RSCDbContext _context;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserRole _userRoleService;
        private readonly ILogger<EditModel> _logger;
        private readonly UserHandler _userHandler;

        public DeleteModel(RSCDbContext context, IUserService userService, IRoleService roleService, IUserRole userRole, ILogger<EditModel> logger, UserHandler userHandler)
        {
            _context = context;
            _userService = userService;
            _roleService = roleService;
            _userRoleService = userRole;
            _logger = logger;
            _userHandler = userHandler;
        }

        [BindProperty]
        public CreateUserCommand Command { get; set; } = default!;
        public SelectList? RolesList { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userService.GetUserBy(id);
            if (user == null)
            {
                return NotFound();
            }

            Command = new CreateUserCommand
            {
                Users = user,
                SelectedRoleId = _userRoleService.GetUserRoleIdByEmailAsync(user.Email).Result ?? 0
            };

            RolesList = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName", Command.SelectedRoleId);

            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            try
            {
                var result = await _userHandler.DeleteHandleAsync(Command.Users!.Email);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred while deleting the user.");

                    // Repopulate RoleList and Company in case of error
                    ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                    RolesList = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName", Command.SelectedRoleId);
                    return Page();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userService.UserExists(Command.Users!.Email))
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
    }
}
