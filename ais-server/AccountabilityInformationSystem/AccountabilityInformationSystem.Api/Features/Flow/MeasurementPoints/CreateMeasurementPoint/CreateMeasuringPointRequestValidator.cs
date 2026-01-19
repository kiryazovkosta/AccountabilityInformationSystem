using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.CreateMeasurementPoint;

internal sealed class CreateMeasuringPointRequestValidator : AbstractValidator<CreateMeasuringPointRequest>
{
    public CreateMeasuringPointRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.NameMaxLength)
            .WithMessage($"'Name' must not exceed {EntitiesConstants.NameMaxLength} characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.FullNameMaxLength)
            .WithMessage($"'Full Name' must not exceed {EntitiesConstants.FullNameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(EntitiesConstants.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ControlPoint)
            .Matches(EntitiesConstants.ControlPointPattern)
            .WithMessage($"'Control Point' must match the pattern {EntitiesConstants.ControlPointPattern}.");

        RuleFor(x => x.FlowDirection)
            .IsInEnum()
            .WithMessage($"'Flow Direction' must be one of the defined enum values.");

        RuleFor(x => x.Transport)
            .IsInEnum()
            .WithMessage($"'Transport' must be one of the defined enum values.");

        RuleFor(x => x.OrderPosition)
            .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
            .WithMessage($"'Order Position' must be greater than or equal to {EntitiesConstants.OrderPositionMinValue}.");

        RuleFor(x => x.ActiveFrom)
            .LessThanOrEqualTo(x => x.ActiveTo)
            .WithMessage("'Active From' must be less than or equal to 'Active To'.");

        RuleFor(x => x.ActiveTo)
            .Must(date => date > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'Active To' must be a future date.");

        RuleFor(x => x.IkunkId)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.IdMaxLength);
    }
}
