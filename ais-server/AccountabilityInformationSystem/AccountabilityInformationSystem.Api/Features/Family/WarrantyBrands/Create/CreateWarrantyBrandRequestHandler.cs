using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using ImTools;
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
                new Error("ExciseNumber", "Warehouse with the same name or excise number already exists!"),
                ResultFailureType.Conflict);
        }

        WarrantyBrand warrantyBrand = request.Adapt<WarrantyBrand>();
        await dbContext.WarrantyBrands.AddAsync(warrantyBrand, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        WarrantyBrandResponse response = warrantyBrand.Adapt<WarrantyBrandResponse>();
        return Result<WarrantyBrandResponse>.Success(response, ResultSuccessType.Created);
    }
}
