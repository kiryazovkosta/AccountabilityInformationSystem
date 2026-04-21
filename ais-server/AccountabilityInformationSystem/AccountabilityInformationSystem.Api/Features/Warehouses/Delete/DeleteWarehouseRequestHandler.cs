using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Delete;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Delete;

public sealed class DeleteWarehouseRequestHandler(ApplicationDbContext dbContext)
{
    public async Task<Result> Handle(DeleteWarehouseRequest request, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await dbContext.Warehouses
            .Include(w => w.Ikunks)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (warehouse is null)
        {
            return Result.Failure(new Error("Id", "Warehouse with specific id does not exist!"), ResultFailureType.NotFound);
        }

        if (warehouse.Ikunks.Count > 0)
        {
            return Result.Failure(new Error("Ikunks", "Cannot delete warehouse with associated ikunks!"), ResultFailureType.BadRequest);
        }

        dbContext.Warehouses.Remove(warehouse);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
