using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadMailValidator : AbstractValidator<UpdateLeadMailRequest>
{
    public UpdateLeadMailValidator()
    {
        RuleFor(r => r.Mail)
            .EmailAddress()
            .WithMessage(LeadsValidators.Mail);
    }
}
