using RoadSignCapture.Core.Models;
using RoadSignCapture.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadSignCapture.Core.Users.Queries
{
    public class GetUserHandler
    {
        private readonly IUserService? _userService;
        public GetUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            return await _userService!.GetUserDetailsBy(email);
        }
    }
}
