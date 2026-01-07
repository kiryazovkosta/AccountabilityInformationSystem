using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Models.ProductTypes;

public static class ProductTypeMappings
{
    public static readonly SortMappingDefinition<ProductTypeResponse, ProductType> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ProductTypeResponse.Name), nameof(ProductType.Name)),
            new SortMapping(nameof(ProductTypeResponse.FullName), nameof(ProductType.FullName)),
        ]
    };


    public static ProductType ToEntity(this CreateProductTypeRequest request, string userName)
        => new()
        {
            Id = $"pt_{Guid.CreateVersion7()}",
            Name = request.Name,
            FullName = request.FullName,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
        };

    public static ProductTypeResponse ToResponse(this ProductType productType)
        => new()
        {
            Id = productType.Id,
            Name = productType.Name,
            FullName = productType.FullName,
            Products = productType.Products.Select(p => new ProductTypeProductResponse()
            {
                Id = p.Id,
                Code = p.Code,
                FullName = p.FullName,
            }).ToList()
        };
}
