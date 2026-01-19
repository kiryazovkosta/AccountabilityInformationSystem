using System.Dynamic;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.CreateMeasurementPointData;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Encrypting;
using AccountabilityInformationSystem.Api.Shared.Services.Linking;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData;

[ApiController]
[Route("api/flow/measuring-points-data")]
[ApiVersion(1.0)]
[Authorize(Roles = $"{Role.Admin},{Role.FlowUser}")]
public class MeasurementPointsDataController(
    ApplicationDbContext dbContext,
    LinkService linkService,
    EncryptionService encryptionService,
    UserContext userContext) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<MeasurementPointDataListResponse>))]
    public async Task<IActionResult> GetMeasuringPointsData(
        [FromQuery] MeasurementPointsDataQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointDataListResponse, MeasurementPointData>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<MeasurementPointDataListResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointDataListResponse, MeasurementPointData>();

        IQueryable<MeasurementPointDataListResponse> measurementPointQuery = dbContext
            .MeasurementPointsData
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.MeasurementPoint.ControlPoint, $"%{query.Search}%") ||
                EF.Functions.Like(mp.MeasurementPoint.FullName, $"%{query.Search}%") ||
                EF.Functions.Like(mp.Product.FullName, $"%{query.Search}%")
            )
            .SetMeasuringPoints(query.Warehouses, query.Ikunks, query.MeasurementPoints)
            .SetTimePeriod(query.Begin, query.End)
            .Where(mpd => query.FlowDirection == null || mpd.FlowDirectionType == query.FlowDirection)
            .Where(mpd => query.Products == null || !query.Products.Any() || query.Products.Contains(mpd.ProductId))
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointDataQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await measurementPointQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await measurementPointQuery
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields,
                query.IncludeLinks ? mp => CreateLinksForMeasuringPointData(mp.Id, query.Fields) : null)
        };
        if (query.IncludeLinks)
        {
            response.Links = CreateLinksForMeasuringPointsData(query, response.HasNextPage, response.HasPreviousPage);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetMeasuringPointData(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<MeasurementPointDataResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        MeasurementPointDataResponse? measuringPointDataResponse = await dbContext
            .MeasurementPointsData
            .AsNoTracking()
            .Select(MeasurementPointDataQueries.ProjectToDatailsResponse())
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (measuringPointDataResponse is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"Not found");
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(measuringPointDataResponse, query.Fields);
        if (query.IncludeLinks)
        {
            shapedResponse.TryAdd("links", CreateLinksForMeasuringPointData(id, query.Fields));
        }

        return Ok(shapedResponse);
    }

    [HttpPost]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> CreateMeasuringPointData(
        [FromBody] CreateMeasuringPointDataRequest request,
        IValidator<CreateMeasuringPointDataRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await dbContext.MeasurementPoints.AnyAsync(i => i.Id == request.MeasurementPointId, cancellationToken))
        {
            return Problem(
                detail: "MeasurementPoint with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (!await dbContext.Products.AnyAsync(i => i.Id == request.ProductId, cancellationToken))
        {
            return Problem(
                detail: "Product with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (await dbContext.MeasurementPointsData.AnyAsync(mpd => mpd.MeasurementPointId == request.MeasurementPointId &&
            mpd.Number == request.Number &&
            mpd.BeginTime == request.BeginTime &&
            mpd.EndTime == request.EndTime &&
            mpd.FlowDirectionType == request.FlowDirectionType, cancellationToken))
        {
            return Problem(
                detail: "Measurement point data with specific parameters already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        MeasurementPointData measuringPointData = request.ToEntity(user.Email);
        measuringPointData.CreatedBy = encryptionService.Encrypt(user.Email);
        await dbContext.MeasurementPointsData.AddAsync(measuringPointData, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        MeasurementPointDataResponse measurementPointResponse = measuringPointData.ToResponse() with
        {
            Links = CreateLinksForMeasuringPointData(measuringPointData.Id)
        };

        return CreatedAtAction(
            actionName: nameof(GetMeasuringPointData),
            routeValues: new { id = measurementPointResponse.Id },
            value: measurementPointResponse);
    }

    private List<LinkResponse> CreateLinksForMeasuringPointData(string id, string? fields = null)
    {
        List<LinkResponse> links =
        [
            linkService.Create(nameof(GetMeasuringPointData), "self", HttpMethods.Get, new { id, fields })
        ];

        return links;
    }

    private List<LinkResponse> CreateLinksForMeasuringPointsData(MeasurementPointsDataQueryParameters query, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkResponse> links =
       [
           linkService.Create(nameof(GetMeasuringPointsData), "self", HttpMethods.Get, new
            {
                page = query.Page,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                warehouses = query.Warehouses,
                ikunks = query.Ikunks,
                measurementpoints = query.MeasurementPoints,
                begin = query.Begin,
                end = query.End,
                flowDirection = query.FlowDirection,
                products = query.Products

            }),
            linkService.Create(nameof(CreateMeasuringPointData), "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetMeasuringPointsData), "next-page", HttpMethods.Get, new
            {
                page = query.Page + 1,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                warehouses = query.Warehouses,
                ikunks = query.Ikunks,
                measurementpoints = query.MeasurementPoints,
                begin = query.Begin,
                end = query.End,
                flowDirection = query.FlowDirection,
                products = query.Products
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(nameof(GetMeasuringPointsData), "previous-page", HttpMethods.Get, new
                {
                    page = query.Page - 1,
                    pageSize = query.PageSize,
                    fields = query.Fields,
                    q = query.Search,
                    sort = query.Sort,
                    warehouses = query.Warehouses,
                    ikunks = query.Ikunks,
                    measurementpoints = query.MeasurementPoints,
                    begin = query.Begin,
                    end = query.End,
                    flowDirection = query.FlowDirection,
                    products = query.Products
                })
            );
        }

        return links;
    }
}

public static class QueryableMeasurementPointDataExtensions
{
    public static IQueryable<MeasurementPointData> SetMeasuringPoints(this IQueryable<MeasurementPointData> query,
        List<string>? warehouses,
        List<string>? ikunks,
        List<string>? measurementPoints)
    {
        if (measurementPoints is not null && measurementPoints.Any())
        {
            query = query.Where(mpd => measurementPoints.Contains(mpd.MeasurementPointId));
        }
        else if (ikunks is not null && ikunks.Any())
        {
            query = query.Where(mpd => ikunks.Contains(mpd.MeasurementPoint.IkunkId));
        }
        else if (warehouses is not null && warehouses.Any())
        {
            query = query.Where(mpd => warehouses.Contains(mpd.MeasurementPoint.Ikunk.WarehouseId));
        }

        return query;
    }

    public static IQueryable<MeasurementPointData> SetTimePeriod(this IQueryable<MeasurementPointData> query,
        DateTime? beginTime,
        DateTime? endTime)
    {
        if (beginTime.HasValue && endTime.HasValue)
        {
            query = query.Where(mpd => mpd.EndTime >= beginTime && mpd.EndTime <= endTime);
        }

        return query;
    }
}
