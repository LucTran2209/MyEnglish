using FastEndpoints;
using FluentValidation;
using MyEnglish.Application.Features.RefreshToken;

namespace MyEnglish.Api.Endpoints.Auth.Validators;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}