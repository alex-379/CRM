using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class LoginLeadValidator : AbstractValidator<LoginLeadRequest>
{
    public LoginLeadValidator()
    {
        RuleFor(r => r.Mail)
            .NotEmpty()
            .WithMessage(LeadsValidators.Mail);
        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage(LeadsValidators.Password);
    }
}
