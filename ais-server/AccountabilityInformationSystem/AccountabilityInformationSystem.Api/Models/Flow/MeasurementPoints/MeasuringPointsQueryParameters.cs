using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Common;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record MeasuringPointsQueryParameters : QueryParameters
{
    public string? IkunkId { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
}

public sealed record MeasurementPointsDataQueryParameters : QueryParameters
{
    public List<string>? Warehouses { get; init; }
    public List<string>? Ikunks { get; init; }
    public List<string>? MeasurementPoints { get; init; }
    public DateTime? Begin { get; init; }
    public DateTime? End { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public List<string>? Products { get; init; }
}
