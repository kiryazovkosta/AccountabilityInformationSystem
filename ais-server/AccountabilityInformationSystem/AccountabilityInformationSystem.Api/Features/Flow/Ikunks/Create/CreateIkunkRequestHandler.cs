using System.ComponentModel.DataAnnotations;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;

public sealed class CreateIkunkRequestHandler(
    ApplicationDbContext dbContext)
{
    public async Task<Result<IkunkResponse>> Handle(CreateIkunkRequest request, CancellationToken cancellationToken)
    {
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

        Ikunk ikunk = request.Adapt<Ikunk>();
        await dbContext.Ikunks.AddAsync(ikunk, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        IkunkResponse ikunkResponse = ikunk.Adapt<IkunkResponse>();
        return Result<IkunkResponse>.Success(ikunkResponse);
    }
}
