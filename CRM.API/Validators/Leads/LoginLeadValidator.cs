using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class LoginLeadValidator : AbstractValidator<LoginLeadRequest>
{
    public LoginLeadValidator()
    {
        RuleFor(r => r.Mail)
            .EmailAddress()
            .WithMessage(string.Format(LeadsValidators.MailLength, DatabaseProperties.MailLength))
            .MaximumLength(DatabaseProperties.MailLength)
            .WithMessage(LeadsValidators.Mail);
        RuleFor(r => r.Password)
            .Length(ValidationSettings.PasswordLength)
            .WithMessage(LeadsValidators.Password);
    }
}
