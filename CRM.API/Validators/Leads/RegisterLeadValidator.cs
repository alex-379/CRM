using CRM.API.Validators.Messages;
using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class RegisterLeadValidator : AbstractValidator<RegisterLeadRequest>
{
    public RegisterLeadValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage(LeadsValidators.Name)
            .MaximumLength(DatabaseProperties.NameLength)
            .WithMessage(string.Format(LeadsValidators.NameLength, DatabaseProperties.NameLength));
        RuleFor(r => r.Mail)
            .NotNull()
            .WithMessage(LeadsValidators.Mail)
            .EmailAddress()
            .WithMessage(LeadsValidators.MailIncorrect)
            .MaximumLength(DatabaseProperties.MailLength)
            .WithMessage(string.Format(LeadsValidators.MailLength, DatabaseProperties.MailLength));
        RuleFor(r => r.Phone)
            .NotNull()
            .WithMessage(LeadsValidators.Phone)
            .MatchPhoneRule();
        RuleFor(r => r.Address)
            .NotNull()
            .WithMessage(LeadsValidators.Address)
            .MaximumLength(DatabaseProperties.AddressLength)
            .WithMessage(string.Format(LeadsValidators.AddressLength, DatabaseProperties.AddressLength));
        RuleFor(r => r.BirthDate)
            .NotEmpty()
            .WithMessage(LeadsValidators.BirthDate)
            .LessThan(AgeSettings.AgeEnd)
            .WithMessage(string.Format(LeadsValidators.BirthDateMax, AgeSettings.AgeEnd.Year))
            .GreaterThan(AgeSettings.AgeStart)
            .WithMessage(string.Format(LeadsValidators.BirthDateMin, AgeSettings.AgeStart.Year));
        RuleFor(r => r.Password)
            .NotNull()
            .WithMessage(LeadsValidators.Password)
            .MatchPasswordRule();
    }
}
