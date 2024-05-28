using CRM.API.Validators.Messages;
using CRM.Business.Models.Accounts.Requests;
using FluentValidation;

namespace CRM.API.Validators.Accounts;

public class RegisterAccountValidator : AbstractValidator<RegisterAccountRequest>
{
    public RegisterAccountValidator()
    {
        RuleFor(r => r.Currency)
            .IsInEnum()
            .WithMessage(AccountsValidators.Currency);
    }
}
