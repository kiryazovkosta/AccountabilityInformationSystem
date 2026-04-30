using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;

public sealed record GetMeasuringPointsRequest : QueryParameters
{
    public string? IkunkId { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
}
