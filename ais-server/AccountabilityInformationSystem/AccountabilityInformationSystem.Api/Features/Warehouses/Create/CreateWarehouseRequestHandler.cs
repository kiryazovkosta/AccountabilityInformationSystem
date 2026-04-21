using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Create;

public sealed class CreateWarehouseRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result<WarehouseResponse>> Handle(CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<WarehouseResponse>.Failure(
                new Error("User", "Non existsing or unauthorized user!"), 
                ResultFailureType.Unauthorized);
        }

        if (await dbContext.Warehouses.AnyAsync(w => w.ExciseNumber == request.ExciseNumber, cancellationToken))
        {
            return Result< WarehouseResponse>.Failure(
                new Error("ExciseNumber", "Warehouse with the same name or excise number already exists!"), 
                ResultFailureType.Conflict);
        }

        Warehouse warehouse = request.ToEntity(user.Email);
        await dbContext.Warehouses.AddAsync(warehouse, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        WarehouseResponse warehouseResponse = warehouse.ToResponse();
        return Result< WarehouseResponse>.Success(warehouseResponse, ResultSuccessType.Created);
    }
}
