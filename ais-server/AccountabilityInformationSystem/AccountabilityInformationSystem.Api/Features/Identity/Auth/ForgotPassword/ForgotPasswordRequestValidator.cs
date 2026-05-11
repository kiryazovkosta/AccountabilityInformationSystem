using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;

internal sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");
    }
}
