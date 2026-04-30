using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;

public sealed record GetMeasuringPointsV2Request : QueryParameters
{
    public string? IkunkId { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
}
