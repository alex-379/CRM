using CRM.API.Validators.Messages;
using CRM.Business.Models.Leads.Requests;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadStatusValidator : AbstractValidator<UpdateLeadStatusRequest>
{
    public UpdateLeadStatusValidator()
    {
        RuleFor(r => r.Status)
            .IsInEnum()
            .WithMessage(LeadsValidators.Status);
    }
}
