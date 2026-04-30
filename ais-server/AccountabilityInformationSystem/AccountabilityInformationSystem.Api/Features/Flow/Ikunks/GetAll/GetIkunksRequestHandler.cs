using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.GetAll;

public sealed class GetIkunksRequestHandler(
    ApplicationDbContext dbContext,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetIkunksRequest request,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<IkunkResponse, Ikunk>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<IkunkResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<IkunkResponse, Ikunk>();

        IQueryable<IkunkResponse> ikunksQuery = dbContext.Ikunks
            .Where(ik =>
                request.Search == null ||
                EF.Functions.Like(ik.Name, $"%{request.Search}%") ||
                EF.Functions.Like(ik.FullName, $"%{request.Search}%") ||
                ik.Description != null && EF.Functions.Like(ik.Description, $"%{request.Search}%"))
            .ApplySort(request.Sort, sortMappings, "OrderPosition")
            .AsNoTracking()
            .ProjectToType<IkunkResponse>();

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await ikunksQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await ikunksQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken),
                request.Fields)
        };

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
