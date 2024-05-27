using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public abstract class RegisterLeadValidator : AbstractValidator<RegisterLeadRequest>
{
    protected RegisterLeadValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage(LeadsValidators.Name);
        RuleFor(r => r.Mail)
            .EmailAddress()
            .WithMessage(LeadsValidators.Mail);
        RuleFor(r => r.Phone)
            .NotEmpty()
            .WithMessage(LeadsValidators.Phone);
        RuleFor(r => r.Address)
            .NotEmpty()
            .WithMessage(LeadsValidators.Address);
        RuleFor(r => r.BirthDate)
            .NotEmpty()
            .WithMessage(LeadsValidators.BirthDate);
        RuleFor(r => r.Password)
            .MatchPasswordRule();
    }
}
