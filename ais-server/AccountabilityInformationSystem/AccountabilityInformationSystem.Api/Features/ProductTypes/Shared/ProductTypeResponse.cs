using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;

public sealed class ProductTypeResponse : IMapFrom<ProductType>
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public ICollection<ProductTypeProductResponse> Products { get; init; } = [];
}
