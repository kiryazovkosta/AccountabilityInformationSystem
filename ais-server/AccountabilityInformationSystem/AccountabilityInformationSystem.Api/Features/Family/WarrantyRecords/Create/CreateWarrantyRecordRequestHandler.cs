using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;

public sealed class CreateWarrantyRecordRequestHandler(
    ApplicationDbContext dbContext,
    UserContext userContext)
{
    public async Task<Result<WarrantyRecordResponse>> Handle(CreateWarrantyRecordRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result<WarrantyRecordResponse>.Failure(
                new Error("User", "Non existing or unauthorized user!"),
                ResultFailureType.Unauthorized);
        }

        bool brandExists = await dbContext.WarrantyBrands
            .AnyAsync(b => b.Id == request.WarrantyBrandId, cancellationToken);

        if (!brandExists)
        {
            return Result<WarrantyRecordResponse>.Failure(
                new Error("WarrantyBrandId", "Warranty brand not found."),
                ResultFailureType.NotFound);
        }

        WarrantyRecord record = request.ToEntity(user.Username);
        await dbContext.WarrantyRecords.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<WarrantyRecordResponse>.Success(record.ToResponse(), ResultSuccessType.Created);
    }
}
