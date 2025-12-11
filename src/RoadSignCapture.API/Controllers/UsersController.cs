using Microsoft.AspNetCore.Authorization;
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
        private readonly FluentValidation.IValidator<UserDto> _userValidator;

        public UsersController(GetUserHandler getUserHandler, FluentValidation.IValidator<UserDto> userValidator)
        {
            _getUserHandler = getUserHandler;
            _userValidator = userValidator;
        }

        // GET: api/users/{email}
        [Authorize]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetDetailsByEmail(string email)
        {
            var validationResult = _userValidator.Validate(new UserDto { Email = email });
            if (!validationResult.IsValid)
            {
              var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var problemDetails = new HttpValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                    Detail = "See the errors property for details.",
                    Instance = HttpContext.Request.Path
                };
                
                return BadRequest(problemDetails);
            }
            var user = await _getUserHandler!.GetUserByEmailAsync(email);
            return user == null ? NotFound() : Ok(user);
        }
    }
}
