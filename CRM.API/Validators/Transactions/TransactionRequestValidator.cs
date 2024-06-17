using CRM.API.Validators.Messages;
using CRM.Business.Models.Transactions.Requests;
using FluentValidation;

namespace CRM.API.Validators.Transactions;

public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
{
    public TransactionRequestValidator()
    {
        RuleFor(r => r.Amount)
            .GreaterThan(0)
            .WithMessage(TransactionsValidators.Amount);
    }
}