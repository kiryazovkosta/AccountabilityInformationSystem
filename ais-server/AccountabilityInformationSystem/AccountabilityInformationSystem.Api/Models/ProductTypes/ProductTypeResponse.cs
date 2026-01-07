using AccountabilityInformationSystem.Api.Models.Products;

namespace AccountabilityInformationSystem.Api.Models.ProductTypes;

public sealed class ProductTypeResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public ICollection<ProductTypeProductResponse> Products { get; init; } = [];
}
