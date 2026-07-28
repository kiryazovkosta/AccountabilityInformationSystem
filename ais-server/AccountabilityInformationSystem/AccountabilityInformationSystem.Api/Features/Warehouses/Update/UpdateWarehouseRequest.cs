using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Update;

public sealed record UpdateWarehouseRequest : IMapTo<Warehouse>, IMapCustom
{
    public string? Name { get; init; }
    public string? FullName { get; init; }
    public string? Description { get; init; }
    public int? OrderPosition { get; init; }
    public string? ExciseNumber { get; init; }
    public DateOnly?  ActiveFrom { get; init; }
    public DateOnly? ActiveTo { get; init; }

    public void CreateMappings(TypeAdapterConfig config) =>
        config.NewConfig<UpdateWarehouseRequest, Warehouse>()
            .IgnoreNullValues(true);
}
