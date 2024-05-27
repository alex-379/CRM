using CRM.Core.Constants;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;
using FluentValidation.Validators;

namespace CRM.API.Validators;

public static class CustomRules
{
    private const string regex = @"(?=^.{{{8},}}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$";
    public static IRuleBuilderOptions<T, string> MatchPasswordRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var passwordRule = ruleBuilder
            .SetValidator(new RegularExpressionValidator<T>(regex))
            .WithMessage(string.Format(LeadsValidators.PasswordRule, ValidationSettings.PasswordLength));

        return passwordRule;
    }
}
