using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Warehouses;

internal sealed class WarehousesModule : IFeatureModule
{
    public void Register(IServiceCollection services) =>
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<WarehouseResponse, Warehouse>>(_ =>
            WarehouseMappings.SortMapping);
}
