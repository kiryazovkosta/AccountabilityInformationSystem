using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Update;

public sealed class UpdateIkunkRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext
    )
{
    public async Task<Result> Handle(UpdateIkunkRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("user", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        bool nameExists = await dbContext
            .Ikunks
            .AnyAsync(ik => ik.Name == request.Name && ik.Id != request.Id, cancellationToken);
        if (nameExists)
        {
            return Result.Failure(
                new Error("id", "Ikunk with specific name already exists!"),
                ResultFailureType.Conflict);
        }

        Ikunk? ikunk = await dbContext
            .Ikunks
            .FirstOrDefaultAsync(ik => ik.Id == request.Id, cancellationToken);
        if (ikunk is null)
        {
            return Result.Failure(
                new Error("id", "Ikunk with specific id does not exists!"),
                ResultFailureType.NotFound);
        }

        if (request.WarehouseId is not null
            && !await dbContext.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken))
        {
            return Result.Failure(
                new Error("id", "Warehouse with specific id does not exists!"));
        }

        ikunk.UpdateFromRequest(request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
