using CRM.Business.Models.Leads.Requests;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadPasswordValidator : AbstractValidator<UpdateLeadPasswordRequest>
{
    public UpdateLeadPasswordValidator()
    {
        RuleFor(r => r.Password)
            .MatchPasswordRule();
    }
}
