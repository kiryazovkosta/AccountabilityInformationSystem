using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;

internal sealed class NewDeviceRequestValidator : AbstractValidator<NewDeviceRequest>
{
    public NewDeviceRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(x => x.RecoveryCode)
            .NotEmpty()
            .WithMessage("Recovery code is required.");
    }
}
