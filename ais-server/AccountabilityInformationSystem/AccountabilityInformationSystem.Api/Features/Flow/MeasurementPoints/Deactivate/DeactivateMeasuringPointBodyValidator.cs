using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

internal sealed class DeactivateMeasuringPointBodyValidator : AbstractValidator<DeactivateMeasuringPointBody>
{
    public DeactivateMeasuringPointBodyValidator()
    {
        RuleFor(x => x.ActiveTo)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Active to date must be today or in the future.");
    }
}
