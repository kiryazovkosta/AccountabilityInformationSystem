using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;

public sealed class GetMeasurementPointsDataRequestHandler(
    ApplicationDbContext dbContext,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    MeasuringPointDataLinkService measuringPointDataLinkService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetMeasurementPointsDataRequest request, 
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointDataListResponse, MeasurementPointData>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<MeasurementPointDataListResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointDataListResponse, MeasurementPointData>();

        IQueryable<MeasurementPointDataListResponse> measurementPointQuery = dbContext
            .MeasurementPointsData
            .Where(mp =>
                request.Search == null ||
                EF.Functions.Like(mp.MeasurementPoint.ControlPoint, $"%{request.Search}%") ||
                EF.Functions.Like(mp.MeasurementPoint.FullName, $"%{request.Search}%") ||
                EF.Functions.Like(mp.Product.FullName, $"%{request.Search}%")
            )
            .SetMeasuringPoints(request.Warehouses, request.Ikunks, request.MeasurementPoints)
            .SetTimePeriod(request.Begin, request.End)
            .Where(mpd => request.FlowDirection == null || mpd.FlowDirectionType == request.FlowDirection)
            .Where(mpd => request.Products == null || !request.Products.Any() || request.Products.Contains(mpd.ProductId))
            .ApplySort(request.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointDataQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await measurementPointQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await measurementPointQuery
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken),
                request.Fields,
                request.IncludeLinks ? mp => measuringPointDataLinkService.CreateLinksForMeasuringPointData(mp.Id, request.Fields) : null)
        };
        if (request.IncludeLinks)
        {
            response.Links = 
                measuringPointDataLinkService.CreateLinksForMeasuringPointsData(
                    request, 
                    response.HasNextPage, 
                    response.HasPreviousPage);
        }

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
