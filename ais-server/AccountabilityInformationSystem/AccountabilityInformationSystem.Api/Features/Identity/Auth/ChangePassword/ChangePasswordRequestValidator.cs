using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(cpr => cpr.OldPassword).NotEmpty().WithMessage("OldPassword is required.");
        RuleFor(cpr => cpr.NewPassword).NotEmpty().WithMessage("NewPassword is required.");
        RuleFor(cpr => cpr.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword is required.");
        RuleFor(cpr => cpr.NewPassword).Equal(cpr => cpr.ConfirmPassword).WithMessage("Passwords must match.");
    }
}
