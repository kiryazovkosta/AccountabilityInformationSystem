using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.GetAll;

public sealed class GetWarrantyBrandsRequestHandler(ApplicationDbContext dbContext)
{
    public async Task<Result<WarrantyBrandsCollectionResponse>> Handle(
        GetWarrantyBrandsRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;

        List<WarrantyBrandResponse> items = await dbContext.WarrantyBrands
            .AsNoTracking()
            .OrderBy(brand => brand.Name)
            .ProjectToType<WarrantyBrandResponse>()
            .ToListAsync(cancellationToken);

        return Result<WarrantyBrandsCollectionResponse>.Success(new WarrantyBrandsCollectionResponse
        {
            Items = items,
        });
    }
}
