using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Accounts;

public class RegistrationAccountValidator : AbstractValidator<RegistrationAccountRequest>
{
    public RegistrationAccountValidator()
    {
        RuleFor(r => r.Currency)
            .IsInEnum()
            .WithMessage(AccountsValidators.Currency);
    }
}
