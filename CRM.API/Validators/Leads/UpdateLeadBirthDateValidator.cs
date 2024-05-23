using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadBirthDateValidator : AbstractValidator<UpdateLeadBirthDateRequest>
{
    public UpdateLeadBirthDateValidator()
    {
        RuleFor(r => r.BirthDate)
            .NotEmpty()
            .WithMessage(LeadsValidators.BirthDate);
    }
}
