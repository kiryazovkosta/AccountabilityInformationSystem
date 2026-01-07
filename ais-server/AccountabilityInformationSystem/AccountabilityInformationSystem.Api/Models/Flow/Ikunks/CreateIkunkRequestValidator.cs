using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

internal sealed class CreateIkunkRequestValidator : AbstractValidator<CreateIkunkRequest>
{
    public CreateIkunkRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.NameMaxLength)
            .WithMessage("Name is required and cannot exceed {MaxLength} characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.FullNameMaxLength)
            .WithMessage("Full Name is required and cannot exceed {MaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(EntitiesConstants.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed {MaxLength} characters.");

        RuleFor(x => x.OrderPosition)
            .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
            .WithMessage($"Order Position must be at least {EntitiesConstants.OrderPositionMinValue}.");

        RuleFor(x => x.ActiveFrom)
            .LessThan(x => x.ActiveTo)
            .WithMessage("'Active From' must be earlier than 'Active To'.");

        RuleFor(x => x.ActiveTo)
            .Must(date => date > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'Active To' must be a future date.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.IdMaxLength);
    }
}
