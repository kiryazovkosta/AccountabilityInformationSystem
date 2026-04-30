using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.GetById;

public sealed class GetProductTypeByIdRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService)
{
    public async Task<Result<ExpandoObject>> Handle(GetProductTypeByIdRequest request, CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<ProductTypeResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        ProductTypeResponse? productTypeResponse = await dbContext
            .ProductTypes
            .AsNoTracking()
            .Select(ProductTypeQueries.ProjectToResponse())
            .FirstOrDefaultAsync(mp => mp.Id == request.Id, cancellationToken);
        if (productTypeResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("Id", "ProductType with provided Id does not exists!"),
                ResultFailureType.NotFound);
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(productTypeResponse, request.Fields);
        return Result<ExpandoObject>.Success(shapedResponse);
    }
}
