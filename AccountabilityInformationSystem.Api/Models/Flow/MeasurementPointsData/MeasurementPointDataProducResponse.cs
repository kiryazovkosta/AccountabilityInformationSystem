namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPointsData;

public sealed record MeasurementPointDataProducResponse
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
