namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record IkunkMeasurementPointResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
}
