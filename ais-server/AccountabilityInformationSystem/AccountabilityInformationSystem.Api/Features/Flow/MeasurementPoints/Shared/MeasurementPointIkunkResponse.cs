using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed class MeasurementPointIkunkResponse : IMapFrom<Ikunk>
{
    public string Id { get; init; }
    public string Name { get; init; }
    public MeasurementPointIkunkWarehouseResponse Warehouse { get; init; }
}

public sealed class MeasurementPointIkunkResponseV2 : IMapFrom<Ikunk>
{
    public string Id { get; init; }
    public string Name { get; init; }
}
