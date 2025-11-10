namespace RoadSignCapture.Web.Validators
{
    using FluentValidation;
    using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
    using RoadSignCapture.Core.Services;
    using RoadSignCapture.Core.Users.Commands;

    public class UserValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserService _userService;
        public UserValidator(IUserService userService)
        {
            _userService = userService;
            // Validation rules for User creation
            RuleFor(user => user.Users!.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .Must(BeUniqueEmail).WithMessage("Email must be unique.");

            RuleFor(user => user.Users!.DisplayName)
                .NotEmpty().WithMessage("Display Name is required.");
        }
        private bool BeUniqueEmail(string email)
        {
            return _userService.UserExists(email);
        }
        
    }
}