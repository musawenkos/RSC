using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadSignCapture.Core.Users.Queries;

namespace RoadSignCapture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GetUserHandler? _getUserHandler;

        public UsersController(GetUserHandler getUserHandler)
        {
            _getUserHandler = getUserHandler;
        }

        // GET: api/users/{email}
        [HttpGet("{email}")]
        public async Task<IActionResult> GetDetailsByEmail(string email)
        {
            var user = await _getUserHandler!.GetUserByEmailAsync(email);
            return user == null ? NotFound() : Ok(user);
        }
    }
}
