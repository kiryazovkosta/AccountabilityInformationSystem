namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataMeasurementPointResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
    public MeasurementPointDataMeasurementPointIkunkResponse Ikunk { get; init; }
}
