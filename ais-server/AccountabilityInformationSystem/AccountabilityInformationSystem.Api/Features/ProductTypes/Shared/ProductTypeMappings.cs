using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;

internal static class ProductTypeMappings
{
    public static readonly SortMappingDefinition<ProductTypeResponse, ProductType> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ProductTypeResponse.Name), nameof(ProductType.Name)),
            new SortMapping(nameof(ProductTypeResponse.FullName), nameof(ProductType.FullName)),
        ]
    };
}
