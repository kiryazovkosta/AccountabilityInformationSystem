using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;

public sealed class CreateIkunkRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result<IkunkResponse>> Handle(CreateIkunkRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<IkunkResponse>.Failure(new Error("user", "Unauthorized"), ResultFailureType.Unauthorized);
        }

        if (!await dbContext.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken))
        {
            return Result<IkunkResponse>.Failure(new Error("warehouseId", "Warehouse with specific id does not exists!"));
        }

        if (await dbContext.Ikunks.AnyAsync(ik => ik.Name == request.Name, cancellationToken))
        {
            return Result<IkunkResponse>.Failure(
                new Error("name", "Ikunk with specific name already exists!"),
                ResultFailureType.Conflict);
        }

        Ikunk ikunk = request.ToEntity(user.Email);
        await dbContext.Ikunks.AddAsync(ikunk, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        IkunkResponse ikunkResponse = ikunk.ToResponse();
        return Result<IkunkResponse>.Success(ikunkResponse);
    }
}
