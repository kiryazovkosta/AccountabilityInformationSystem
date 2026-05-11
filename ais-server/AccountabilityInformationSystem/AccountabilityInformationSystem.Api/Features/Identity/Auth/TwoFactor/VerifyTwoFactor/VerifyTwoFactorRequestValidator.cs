using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;

internal sealed class VerifyTwoFactorRequestValidator : AbstractValidator<VerifyTwoFactorRequest>
{
    public VerifyTwoFactorRequestValidator()
    {
        RuleFor(x => x.SetupToken)
            .NotEmpty()
            .WithMessage("Setup token is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Verification code is required.");
    }
}
