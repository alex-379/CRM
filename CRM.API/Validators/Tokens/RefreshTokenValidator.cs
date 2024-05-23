using CRM.Business.Models.Tokens.Requests;
using CRM.Core.Constants.ValidatorsMessages;
using FluentValidation;

namespace CRM.API.Validators.Tokens;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.AccessToken)
            .NotEmpty()
            .WithMessage(TokensValidators.AccessToken);
        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage(TokensValidators.RefreshToken);
    }
}
