using CRM.API.Validators.Messages;
using CRM.Core.Constants;
using FluentValidation;
using FluentValidation.Validators;

namespace CRM.API.Validators.Leads;

public static class CustomRules
{
    private const string regexPassword = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    private const string regexPhone = @"((\(\d{3}\) ?)|(\d{3}))?\d{7}";
    
    public static IRuleBuilderOptions<T, string> MatchPasswordRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var passwordRule = ruleBuilder
            .SetValidator(new RegularExpressionValidator<T>(regexPassword))
            .WithMessage(string.Format(LeadsValidators.PasswordRule, ValidationSettings.PasswordLength));

        return passwordRule;
    }
    
    public static IRuleBuilderOptions<T, string> MatchPhoneRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var phoneRule = ruleBuilder
            .SetValidator(new RegularExpressionValidator<T>(regexPhone))
            .WithMessage(LeadsValidators.PhoneRule);

        return phoneRule;
    }
}
