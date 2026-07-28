using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

public sealed record WarehouseIkunkMeasurementPointResponse : IMapFrom<MeasurementPoint>
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
}
