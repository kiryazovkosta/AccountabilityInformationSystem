using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.GetById;

public sealed class GetWarehouseByIdRequestHandler(
    ApplicationDbContext dbContext,
    DataShapingService dataShapingService
    )
{
    public async Task<Result<ExpandoObject>> Handle(GetWarehouseByIdRequest request, CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<WarehouseResponse>(request.Fields))
        {
            return Result<ExpandoObject>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"),
                ResultFailureType.BadRequest);
        }

        WarehouseResponse? warehouseResponse = await dbContext
            .Warehouses
            .AsNoTracking()
            .Select(WarehouseQueries.ProjectToResponse())
            .FirstOrDefaultAsync(warehouse => warehouse.Id == request.Id, cancellationToken);
        if (warehouseResponse is null)
        {
            return Result<ExpandoObject>.Failure(
                new Error("id", "Warehouse with provided id does not exists!"),
                ResultFailureType.NotFound);
        }

        ExpandoObject shapedData = dataShapingService.ShapeData(warehouseResponse, request.Fields);
        return Result<ExpandoObject>.Success(shapedData);
    }
}
