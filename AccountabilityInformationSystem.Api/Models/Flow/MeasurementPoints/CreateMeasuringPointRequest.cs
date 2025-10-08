using AccountabilityInformationSystem.Api.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record CreateMeasuringPointRequest
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public string? Description { get; init; }
    public required string ControlPoint { get; init; }
    public required int OrderPosition { get; init; }
    public required FlowDirectionType FlowDirection { get; init; }
    public required TransportType Transport { get; init; }
    public required DateOnly ActiveFrom { get; init; }
    public required DateOnly ActiveTo { get; init; }
    public string IkunkId { get; init; }
}
