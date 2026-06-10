using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;

public sealed record WarrantyBrandsCollectionResponse : ICollectionResponse<WarrantyBrandResponse>
{
    public List<WarrantyBrandResponse> Items { get; init; }
}
