namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

public sealed record DeactivateMeasuringPointBody
{
    public required DateOnly ActiveTo { get; init; }
}
