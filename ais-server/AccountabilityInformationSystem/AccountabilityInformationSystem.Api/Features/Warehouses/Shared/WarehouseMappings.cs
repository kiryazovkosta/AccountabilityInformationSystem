using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

internal static class WarehouseMappings
{
    public static readonly SortMappingDefinition<WarehouseResponse, Warehouse> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(WarehouseResponse.Id), nameof(Warehouse.Id)),
            new SortMapping(nameof(WarehouseResponse.Name), nameof(Warehouse.Name)),
            new SortMapping(nameof(WarehouseResponse.FullName), nameof(Warehouse.FullName)),
            new SortMapping(nameof(WarehouseResponse.Description), nameof(Warehouse.Description)),
            new SortMapping(nameof(WarehouseResponse.OrderPosition), nameof(Warehouse.OrderPosition)),
            new SortMapping(nameof(WarehouseResponse.ExciseNumber), nameof(Warehouse.ExciseNumber)),
            new SortMapping(nameof(WarehouseResponse.ActiveFrom), nameof(Warehouse.ActiveFrom)),
            new SortMapping(nameof(WarehouseResponse.ActiveTo), nameof(Warehouse.ActiveTo))
        ]
    };
}
