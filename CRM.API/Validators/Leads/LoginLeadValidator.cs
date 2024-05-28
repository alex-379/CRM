using CRM.API.Validators.Messages;
using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class LoginLeadValidator : AbstractValidator<LoginLeadRequest>
{
    public LoginLeadValidator()
    {
        RuleFor(r => r.Mail)
            .NotNull()
            .WithMessage(LeadsValidators.Mail)
            .EmailAddress()
            .WithMessage(LeadsValidators.MailIncorrect)
            .MaximumLength(DatabaseProperties.MailLength)
            .WithMessage(string.Format(LeadsValidators.MailLength, DatabaseProperties.MailLength));
        RuleFor(r => r.Password)
            .NotNull()
            .WithMessage(LeadsValidators.Password)
            .MinimumLength(ValidationSettings.PasswordLength)
            .WithMessage(string.Format(LeadsValidators.PasswordLength, ValidationSettings.PasswordLength));
    }
}
