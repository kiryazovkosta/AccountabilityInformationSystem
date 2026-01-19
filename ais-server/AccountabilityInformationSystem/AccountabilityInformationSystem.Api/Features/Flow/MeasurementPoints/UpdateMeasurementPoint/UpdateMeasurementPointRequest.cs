using AccountabilityInformationSystem.Api.Domain.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.UpdateMeasurementPoint;

public sealed record UpdateMeasurementPointRequest
{
    public string? Name { get; init; }
    public string? FullName { get; init; }
    public string? Description { get; init; }
    public string? ControlPoint { get; init; }
    public int? OrderPosition { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
    public DateOnly? ActiveFrom { get; init; }
    public DateOnly? ActiveTo { get; init; }
    public string? IkunkId { get; init; }
}
