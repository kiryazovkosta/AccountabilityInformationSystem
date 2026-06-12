using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.GetAll;

public sealed class GetWarrantyRecordsRequestHandler(
    ApplicationDbContext context,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetWarrantyRecordsRequest request,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<WarrantyRecordListResponse, WarrantyRecord>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<WarrantyRecordListResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<WarrantyRecordListResponse, WarrantyRecord>();

        IQueryable<WarrantyRecordListResponse> warrantyRecordsQuery = context.WarrantyRecords
            .Where(wr =>
                request.Search == null ||
                EF.Functions.Like(wr.WarrantyBrand.Name, $"%{request.Search}%") ||
                EF.Functions.Like(wr.Model, $"%{request.Search}%"))
            .ApplySort(request.Sort, sortMappings)
            .AsNoTracking()
            .ProjectToType<WarrantyRecordListResponse>();

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await warrantyRecordsQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await warrantyRecordsQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken),
                request.Fields)
        };

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
