using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataMeasurementPointIkunkWarehouseResponse : IMapFrom<Warehouse>
{
    public string Id { get; init; }
    public string FullName { get; set; }
}
