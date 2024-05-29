using CRM.API.Validators.Messages;
using CRM.Business.Models.Accounts.Requests;
using FluentValidation;

namespace CRM.API.Validators.Accounts;

public class UpdateAccountStatusValidator : AbstractValidator<UpdateAccountStatusRequest>
{
    public UpdateAccountStatusValidator()
    {
        RuleFor(r => r.Status)
            .IsInEnum()
            .WithMessage(AccountsValidators.Status);
    }
}
