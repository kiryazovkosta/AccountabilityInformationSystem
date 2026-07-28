using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataMeasurementPointResponse : IMapFrom<MeasurementPoint>
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
    public MeasurementPointDataMeasurementPointIkunkResponse Ikunk { get; init; }
}
