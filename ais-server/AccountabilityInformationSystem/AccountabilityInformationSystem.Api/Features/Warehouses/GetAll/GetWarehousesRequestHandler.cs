using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.GetAll;

public sealed class GetWarehousesRequestHandler(
    ApplicationDbContext dbContext,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(GetWarehousesRequest request, CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<WarehouseResponse, Warehouse>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<WarehouseResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<WarehouseResponse, Warehouse>();

        IQueryable<WarehouseResponse> warehousesQuery = dbContext
            .Warehouses
            .Where(mp =>
                request.Search == null ||
                EF.Functions.Like(mp.Name, $"%{request.Search}%") ||
                EF.Functions.Like(mp.FullName, $"%{request.Search}%") ||
                EF.Functions.Like(mp.ExciseNumber, $"%{request.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{request.Search}%")
            )
            .ApplySort(request.Sort, sortMappings)
            .AsNoTracking()
            .Select(WarehouseQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await warehousesQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await warehousesQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken),
                request.Fields)
        };

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
