using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace RoadSignCapture.Web.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class CreateModel : PageModel
    {
        private readonly RSCDbContext _context;
        private readonly UserHandler _userHandler;
        private readonly ILogger<CreateModel> _logger;
        private readonly IRoleService _roleService;
        private readonly FluentValidation.IValidator<CreateUserCommand> _userValidator;

        public CreateModel(RSCDbContext context, UserHandler userHandler, IRoleService roleService, ILogger<CreateModel> logger, FluentValidation.IValidator<CreateUserCommand> userValidator)
        {
            _context = context;
            _userHandler = userHandler;
            _roleService = roleService;
            _logger = logger;
            _userValidator = userValidator;
        }


        [BindProperty]
        public CreateUserCommand Command { get; set; } = default!;
        public SelectList? RoleList { get; set; }

        public async Task<IActionResult> OnGet()
        {
            //TODO: Remove the company context
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            RoleList = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            //Validation of Command
            var validationResult = await _userValidator.ValidateAsync(Command);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                // Repopulate RoleList and Company in case of error
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                RoleList = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName");
                return Page();
            }

            var result = await _userHandler.CreateHandleAsync(Command);

            if(!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred while creating the user.");
                // Repopulate RoleList and Company in case of error
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                RoleList = new SelectList(await _roleService.GetAllAsync(), "RoleId", "RoleName");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
