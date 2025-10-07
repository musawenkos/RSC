using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Users.Commands
{
    public class UserHandler
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserRole _userRoleService;

        public UserHandler(IUserService userService, IRoleService roleService, IUserRole userRole)
        {
            _userService = userService;
            _roleService = roleService;
            _userRoleService = userRole;
        }

        public async Task<Result> CreateHandleAsync(CreateUserCommand command)
        {
            if (command.SelectedRoleId == 0)
                return Result.Failure("Please select a role.");

            var role = await _roleService.GetByIdAsync(command.SelectedRoleId);
            if (role == null)
                return Result.Failure("Selected role is invalid.");

            var user = command.Users;

            user!.Roles.Add(role);

            await _userService.AddAsync(user);
            await _userService.SaveChangesAsync();

            return Result.SuccessResult();
        }

        public async Task<Result> EditHandleAsync(CreateUserCommand command,string existingEmail)
        {
            if (command.SelectedRoleId == 0)
                return Result.Failure("Please select a role.");

            var role = await _roleService.GetByIdAsync(command.SelectedRoleId);
            if (role == null)
                return Result.Failure("Selected role is invalid.");

            var existingUser = await _userService.GetUserBy(existingEmail);
            if (existingUser == null)
                return Result.Failure("User not found.");

            // Update user properties
            existingUser.DisplayName = command.Users!.DisplayName;
            existingUser.CompanyId = command.Users!.CompanyId;
            
            var isUpdatedRole = await _userRoleService.UpdateUserRoleByEmailAsync(existingUser.Email, command.SelectedRoleId);
            if ((bool)!isUpdatedRole)
                return Result.Failure("Failed to update user role.");

            await _userService.SaveChangesAsync();

            return Result.SuccessResult();
        }

        //Delete user handle
        public async Task<Result> DeleteHandleAsync(string email)
        {
            var user = await _userService.GetUserBy(email);
            if (user == null)
                return Result.Failure("User not found.");
            // Assuming a method to remove user exists in IUserService
            _userService.Remove(user);
            await _userService.SaveChangesAsync();
            return Result.SuccessResult();
        }

        public record Result(bool Success, string? Error = null)
        {
            public static Result SuccessResult()
            {
                return new(true);
            }

            public static Result Failure(string error) => new(false, error);
        }
    }
}
