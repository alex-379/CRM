using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadDataValidator : AbstractValidator<UpdateLeadDataRequest>
{
    public UpdateLeadDataValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage(LeadsValidators.Name);
        RuleFor(r => r.Phone)
            .NotEmpty()
            .WithMessage(LeadsValidators.Phone);
        RuleFor(r => r.Address)
            .NotEmpty()
            .WithMessage(LeadsValidators.Address);
    }
}
