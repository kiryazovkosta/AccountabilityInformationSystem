using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;

public sealed class GetMeasuringPointsRequestHandler(
    ApplicationDbContext dbContext,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    MeasuringPointLinkService measuringPointLinkService)
{
    public async Task<Result<PaginationResponse<ExpandoObject>>> Handle(
        GetMeasuringPointsRequest request, 
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointResponse, MeasurementPoint>(request.Sort))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("sort", $"Invalid sort parameter. {request.Sort}"));
        }

        if (!dataShapingService.Validate<MeasurementPointResponse>(request.Fields))
        {
            return Result<PaginationResponse<ExpandoObject>>.Failure(
                new Error("fields", $"Invalid fields parameter. {request.Fields}"));
        }

        request.Search = request.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointResponse, MeasurementPoint>();

        IQueryable<MeasurementPointResponse> measurementPointQuery = dbContext
            .MeasurementPoints
            .Where(mp =>
                request.Search == null ||
                EF.Functions.Like(mp.Name, $"%{request.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{request.Search}%")
            )
            .Where(mp => request.IkunkId == null || mp.Ikunk.Id == request.IkunkId)
            .Where(mp => request.FlowDirection == null || mp.FlowDirection == request.FlowDirection)
            .Where(mp => request.Transport == null || mp.Transport == request.Transport)
            .ApplySort(request.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponse());

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
                request.IncludeLinks ? mp => measuringPointLinkService.CreateLinksForMeasuringPoint(mp.Id, request.Fields) : null)
        };
        if (request.IncludeLinks)
        {
            response.Links = measuringPointLinkService.CreateLinksForMeasuringPoints(request, response.HasNextPage, response.HasPreviousPage);
        }

        return Result<PaginationResponse<ExpandoObject>>.Success(response);
    }
}
