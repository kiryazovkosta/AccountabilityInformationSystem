using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed class MeasurementPointIkunkWarehouseResponse : IMapFrom<Warehouse>
{
    public string Id { get; init; }
    public string Name { get; init; }
}
