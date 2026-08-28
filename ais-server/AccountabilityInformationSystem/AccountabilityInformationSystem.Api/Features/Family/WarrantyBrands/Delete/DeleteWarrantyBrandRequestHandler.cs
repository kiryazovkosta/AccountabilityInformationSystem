using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Delete;

public sealed class DeleteWarrantyBrandRequestHandler(ApplicationDbContext dbContext)
{
    public async Task<Result> Handle(DeleteWarrantyBrandRequest request, CancellationToken cancellationToken)
    {
        WarrantyBrand? brand = await dbContext.WarrantyBrands
            .FirstOrDefaultAsync(wb => wb.Id == request.Id, cancellationToken);

        if (brand is null)
        {
            return Result.Failure(new Error("Id", "Warranty brand with specific id does not exist!"), ResultFailureType.NotFound);
        }

        bool isInUse = await dbContext.WarrantyRecords.AnyAsync(wr => wr.WarrantyBrandId == request.Id, cancellationToken);
        if (isInUse)
        {
            return Result.Failure(
                new Error("Id", "This warranty brand is still used by existing warranty records and cannot be deleted."),
                ResultFailureType.Conflict);
        }

        dbContext.WarrantyBrands.Remove(brand);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
