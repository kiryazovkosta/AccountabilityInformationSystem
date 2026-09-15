using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

internal sealed class DeactivateMeasuringPointRequestValidator : AbstractValidator<DeactivateMeasuringPointRequest>
{
    public DeactivateMeasuringPointRequestValidator()
    {
        RuleFor(x => x.ActiveTo)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Active to date must be today or in the future.");
    }
}
