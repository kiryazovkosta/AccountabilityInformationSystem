using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;

public sealed class CreateWarrantyBrandRequestHandler(
    ApplicationDbContext dbContext)
{
    public async Task<Result<WarrantyBrandResponse>> Handle(
        CreateWarrantyBrandRequest request, 
        CancellationToken cancellationToken)
    {
        bool alreadyExists = await dbContext.WarrantyBrands.AnyAsync(rb => rb.Name  == request.Name, cancellationToken);
        if (alreadyExists)
        {
            return Result<WarrantyBrandResponse>.Failure(
                new Error("Name", "WarrantyBrand with the same name already exists!"),
                ResultFailureType.Conflict);
        }

        WarrantyBrand warrantyBrand = request.Adapt<WarrantyBrand>();
        await dbContext.WarrantyBrands.AddAsync(warrantyBrand, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        WarrantyBrandResponse response = warrantyBrand.Adapt<WarrantyBrandResponse>();
        return Result<WarrantyBrandResponse>.Success(response, ResultSuccessType.Created);
    }
}
