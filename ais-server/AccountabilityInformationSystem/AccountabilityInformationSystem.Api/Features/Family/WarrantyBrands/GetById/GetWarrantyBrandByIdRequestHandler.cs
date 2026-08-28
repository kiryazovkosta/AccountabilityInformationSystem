using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.FileStoraging;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.GetById;

public sealed class GetWarrantyBrandByIdRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService)
    {
        public async Task<Result<ExpandoObject>> Handle(
            GetWarrantyBrandByIdRequest request,
            CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<WarrantyBrandResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        WarrantyBrand? record = await dbContext.WarrantyBrands
            .FirstOrDefaultAsync(wb => wb.Id == request.Id, cancellationToken);

        if (record is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "No matching records were found for your search criteria."),
                ResultFailureType.NotFound);
        }

        WarrantyBrandResponse response = record.Adapt<WarrantyBrandResponse>();
        return Result<ExpandoObject>.Success(dataShapingService.ShapeData(response, request.Fields));
    }
}
