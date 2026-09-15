namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

public sealed record DeactivateMeasuringPointRequest
{
    internal string Id { get; init; }
    public required DateOnly ActiveTo { get; init; }
}
