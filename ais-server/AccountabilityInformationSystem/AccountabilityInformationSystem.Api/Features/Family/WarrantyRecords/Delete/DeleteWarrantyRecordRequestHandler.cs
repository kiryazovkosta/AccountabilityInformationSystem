using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Delete;

public sealed class DeleteWarrantyRecordRequestHandler(ApplicationDbContext dbContext, IFileStorage fileStorage)
{
    public async Task<Result> Handle(DeleteWarrantyRecordRequest request, CancellationToken cancellationToken)
    {
        WarrantyRecord? record = await dbContext.WarrantyRecords
            .Include(w => w.Receipt)
            .Include(w => w.FrontImage)
            .Include(w => w.BackImage)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (record is null)
        {
            return Result.Failure(new Error("Id", "Warranty record with specific id does not exist!"), ResultFailureType.NotFound);
        }

        StorageFile?[] files = [record.Receipt, record.FrontImage, record.BackImage];
        foreach (StorageFile? file in files)
        {
            if (file is null)
            {
                continue;
            }

            await fileStorage.DeleteAsync(file.BlobName, file.IsPrivate, cancellationToken);
            dbContext.StorageFiles.Remove(file);
        }

        dbContext.WarrantyRecords.Remove(record);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
