using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;

public sealed record GetMeasurementPointsDataRequest : QueryParameters
{
    public List<string>? Warehouses { get; init; }
    public List<string>? Ikunks { get; init; }
    public List<string>? MeasurementPoints { get; init; }
    public DateTime? Begin { get; init; }
    public DateTime? End { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public List<string>? Products { get; init; }
}
