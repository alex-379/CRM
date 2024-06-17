using CRM.API.Validators.Messages;
using CRM.Business.Models.Transactions.Requests;
using FluentValidation;

namespace CRM.API.Validators.Transactions;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(r => r.Amount)
            .GreaterThan(0)
            .WithMessage(TransactionsValidators.Amount);
    }
}