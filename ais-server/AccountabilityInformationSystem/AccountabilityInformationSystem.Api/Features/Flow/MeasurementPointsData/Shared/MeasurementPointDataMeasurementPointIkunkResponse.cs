using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataMeasurementPointIkunkResponse : IMapFrom<Ikunk>
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public MeasurementPointDataMeasurementPointIkunkWarehouseResponse Warehouse { get; init; }
}
