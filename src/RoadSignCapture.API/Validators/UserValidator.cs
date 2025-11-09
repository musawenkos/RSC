namespace RoadSignCapture.API.Validators
{
    using FluentValidation;
    using RoadSignCapture.Core.Users.Queries;

    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}