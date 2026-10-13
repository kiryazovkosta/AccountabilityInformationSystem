using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;

public sealed class SetupTwoFactorRequestValidator : AbstractValidator<SetupTwoFactorRequest>
{
    public SetupTwoFactorRequestValidator()
    {
        RuleFor(x => x.SetupToken)
            .NotEmpty()
            .WithMessage("Setup token is required.");
    }
}
