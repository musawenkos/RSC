namespace RoadSignCapture.API.Validators
{
    using FluentValidation;
    using RoadSignCapture.Core.Services;
    using RoadSignCapture.Core.Companies.Commands;

    public class CompanyValidator : AbstractValidator<CompanyCommands>
    {
        private readonly ICompanyService _companyService;

        public CompanyValidator(ICompanyService companyService)
        {
            _companyService = companyService;
            RuleFor(company => company.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .Must(BeUniqueCompanyName).WithMessage("Company name already exists. Please choose a different name.");

        }
        
        private bool BeUniqueCompanyName(string companyName)
        {
            return !_companyService.CompanyExists(companyName);
        }
    }
}