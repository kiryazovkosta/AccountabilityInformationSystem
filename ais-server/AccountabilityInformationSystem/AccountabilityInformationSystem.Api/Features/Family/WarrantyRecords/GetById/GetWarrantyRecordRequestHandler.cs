using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.GetById;

public sealed class GetWarrantyRecordRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService,
    IFileStorage fileStorage)
{
    public async Task<Result<ExpandoObject>> Handle(
        GetWarrantyRecordRequest request,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<WarrantyRecordResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        WarrantyRecord? record = await dbContext.WarrantyRecords
            .AsNoTracking()
            .Include(wr => wr.WarrantyBrand)
            .Include(wr => wr.Receipt)
            .Include(wr => wr.FrontImage)
            .Include(wr => wr.BackImage)
            .FirstOrDefaultAsync(wr => wr.Id == request.Id, cancellationToken);

        if (record is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "No matching records were found for your search criteria."),
                ResultFailureType.NotFound);
        }

        WarrantyRecordResponse response = record.ToResponse().FillFileUrls(record, fileStorage);

        return Result<ExpandoObject>.Success(dataShapingService.ShapeData(response, request.Fields));
    }
}
