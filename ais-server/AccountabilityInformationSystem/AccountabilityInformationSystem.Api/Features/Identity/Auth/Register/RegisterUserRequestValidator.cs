using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;

internal sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(EntitiesConstants.UsernameMaxLength)
            .WithMessage($"Username must not exceed {EntitiesConstants.UsernameMaxLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .MaximumLength(EntitiesConstants.EmailMaxLength)
            .WithMessage($"Email must not exceed {EntitiesConstants.EmailMaxLength} characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(EntitiesConstants.FirstNameMaxLength)
            .WithMessage($"First name must not exceed {EntitiesConstants.FirstNameMaxLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(EntitiesConstants.LastNameMaxLength)
            .WithMessage($"Last name must not exceed {EntitiesConstants.LastNameMaxLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}
