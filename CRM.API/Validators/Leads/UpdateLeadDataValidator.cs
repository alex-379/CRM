using CRM.API.Validators.Messages;
using CRM.Business.Models.Leads.Requests;
using CRM.Core.Constants;
using FluentValidation;

namespace CRM.API.Validators.Leads;

public class UpdateLeadDataValidator : AbstractValidator<UpdateLeadDataRequest>
{
    public UpdateLeadDataValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage(LeadsValidators.Name)
            .MaximumLength(DatabaseProperties.NameLength)
            .WithMessage(string.Format(LeadsValidators.NameLength, DatabaseProperties.NameLength));
        RuleFor(r => r.Phone)
            .NotNull()
            .WithMessage(LeadsValidators.Phone)
            .MatchPhoneRule();
        RuleFor(r => r.Address)
            .NotNull()
            .WithMessage(LeadsValidators.Address)
            .MaximumLength(DatabaseProperties.AddressLength)
            .WithMessage(string.Format(LeadsValidators.AddressLength, DatabaseProperties.AddressLength));
    }
}
