using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.ProductTypes.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.GetAll;

public sealed class GetProductTypesRequestHandler(
    ApplicationDbContext dbContext,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetProductTypesRequest request, 
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<ProductTypeResponse, ProductType>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<ProductTypeResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<ProductTypeResponse, ProductType>();

        IQueryable<ProductTypeResponse> productTypeQuery = dbContext
            .ProductTypes
            .Where(pt =>
                request.Search == null ||
                EF.Functions.Like(pt.Name, $"%{request.Search}%") ||
                EF.Functions.Like(pt.FullName, $"%{request.Search}%")
            )
            .ApplySort(request.Sort, sortMappings)
            .AsNoTracking()
            .Select(ProductTypeQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await productTypeQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await productTypeQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken),
                request.Fields)
        };

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
