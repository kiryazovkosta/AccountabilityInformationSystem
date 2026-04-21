using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Update;

public sealed class UpdateWarehouseRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result> Handle(UpdateWarehouseCommand command, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User", "Non existing or unauthorized user!"), ResultFailureType.Unauthorized);
        }

        if (command.Request.ExciseNumber is not null &&
            await dbContext.Warehouses.AnyAsync(
                w => w.ExciseNumber == command.Request.ExciseNumber 
                    && w.Id != command.Id, cancellationToken))
        {
            return Result.Failure(
                new Error("ExciseNumber", "Warehouse with the same excise number already exists!"), 
                ResultFailureType.Conflict);
        }

        Warehouse? warehouse = await dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == command.Id, cancellationToken);
        if (warehouse is null)
        {
            return Result.Failure(new Error("Id", "Warehouse with specific id does not exist!"), ResultFailureType.NotFound);
        }

        warehouse.UpdateFromRequest(command.Request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
