using CRM.API.Validators.Messages;
using CRM.Business.Models.Leads.Requests;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadBirthDateValidator : AbstractValidator<UpdateLeadBirthDateRequest>
{
    public UpdateLeadBirthDateValidator()
    {
        RuleFor(r => r.BirthDate)
            .NotEmpty()
            .WithMessage(LeadsValidators.BirthDate)
            .LessThan(AgeSettings.AgeEnd)
            .WithMessage(LeadsValidators.BirthDateMin)
            .GreaterThan(AgeSettings.AgeStart)
            .WithMessage(LeadsValidators.BirthDateMax);
    }
}
