using CRM.Business.Models.Accounts.Requests;
using CRM.Core.Constants.ValidatorsMessages;
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
