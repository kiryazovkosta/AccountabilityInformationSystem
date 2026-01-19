using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.UpdateMeasurementPoint;

internal sealed class UpdateMeasurementPointRequestValidator
    : AbstractValidator<UpdateMeasurementPointRequest>
{
    public UpdateMeasurementPointRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(EntitiesConstants.NameMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage($"'Name' must not exceed {EntitiesConstants.NameMaxLength} characters.");

        RuleFor(x => x.FullName)
            .MaximumLength(EntitiesConstants.FullNameMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName))
            .WithMessage($"'Full Name' must not exceed {EntitiesConstants.FullNameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(EntitiesConstants.DescriptionMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage($"'Description' must not exceed {EntitiesConstants.DescriptionMaxLength} characters.");

        RuleFor(x => x.ControlPoint)
            .Matches(EntitiesConstants.ControlPointPattern)
            .When(x => !string.IsNullOrEmpty(x.ControlPoint))
            .WithMessage($"'Control Point' must match the pattern {EntitiesConstants.ControlPointPattern}.");

        RuleFor(x => x.FlowDirection)
            .IsInEnum()
            .When(x => x.FlowDirection.HasValue)
            .WithMessage($"'Flow Direction' must be one of the defined enum values.");

        RuleFor(x => x.Transport)
            .IsInEnum()
            .When(x => x.Transport.HasValue)
            .WithMessage($"'Transport' must be one of the defined enum values.");

        RuleFor(x => x.OrderPosition)
            .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
            .When(x => x.OrderPosition.HasValue)
            .WithMessage($"'Order Position' must be greater than or equal to {EntitiesConstants.OrderPositionMinValue}.");

        RuleFor(x => x)
            .Must(x => !x.ActiveFrom.HasValue || !x.ActiveTo.HasValue || x.ActiveFrom <= x.ActiveTo)
            .WithMessage("'Active From' must be less than or equal to 'Active To'.");

        RuleFor(x => x.ActiveTo)
            .Must(date => !date.HasValue || date > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("'Active To' must be a future date.");

        RuleFor(x => x.IkunkId)
            .MaximumLength(EntitiesConstants.IdMaxLength)
            .When(x => !string.IsNullOrEmpty(x.IkunkId))
            .WithMessage($"'IkunkId' must not exceed {EntitiesConstants.IdMaxLength} characters.");
    }
}
