using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Create;

public sealed class CreateWarehouseRequestHandler(
    ApplicationDbContext dbContext)
{
    public async Task<Result<WarehouseResponse>> Handle(CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        if (await dbContext.Warehouses.AnyAsync(w => w.ExciseNumber == request.ExciseNumber, cancellationToken))
        {
            return Result< WarehouseResponse>.Failure(
                new Error("ExciseNumber", "Warehouse with the same name or excise number already exists!"), 
                ResultFailureType.Conflict);
        }

        Warehouse warehouse = request.Adapt<Warehouse>();
        await dbContext.Warehouses.AddAsync(warehouse, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        WarehouseResponse warehouseResponse = warehouse.Adapt<WarehouseResponse>();
        return Result< WarehouseResponse>.Success(warehouseResponse, ResultSuccessType.Created);
    }
}
