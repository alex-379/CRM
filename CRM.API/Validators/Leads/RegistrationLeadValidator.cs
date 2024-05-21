using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class RegistrationLeadValidator : AbstractValidator<RegistrationLeadRequest>
{
    public RegistrationLeadValidator()
    {
        RuleFor(r => r.Name)
        .NotEmpty()
           .WithMessage(LeadsValidators.Name);
        RuleFor(r => r.Mail)
        .EmailAddress()
            .WithMessage(LeadsValidators.Mail);
        RuleFor(r => r.Password)
            .MatchPasswordRule();
    }
}
