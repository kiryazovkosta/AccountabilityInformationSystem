using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes;

internal sealed class ProductTypesModule : IFeatureModule
{
    public void Register(IServiceCollection services) =>
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<ProductTypeResponse, ProductType>>(_ =>
            ProductTypeMappings.SortMapping);
}
