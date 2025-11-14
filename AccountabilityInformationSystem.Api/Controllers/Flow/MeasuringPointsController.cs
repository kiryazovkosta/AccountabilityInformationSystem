using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Net.Mime;
using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models;
using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Services.DataShaping;
using AccountabilityInformationSystem.Api.Services.Linking;
using AccountabilityInformationSystem.Api.Services.Sorting;
using AccountabilityInformationSystem.Api.Services.UserContexting;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AccountabilityInformationSystem.Api.Controllers.Flow;

[ApiController]
[Route("api/flow/measuring-points")]
[ApiVersion(1.0)]
[Authorize(Roles = $"{Role.Admin},{Role.FlowUser}")]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.JsonV2,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1,
    CustomMediaTypeNames.Application.HateoasJsonV2)]
public sealed class MeasuringPointsController(
    ApplicationDbContext dbContext,
    LinkService linkService,
    UserContext userContext) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<MeasurementPointResponse>))]
    public async Task<IActionResult> GetMeasuringPoints(
        [FromQuery] MeasuringPointsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointResponse, MeasurementPoint>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<MeasurementPointResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointResponse, MeasurementPoint>();

        IQueryable<MeasurementPointResponse> measurementPointQuery = dbContext
            .MeasurementPoints
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.Name, $"%{query.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{query.Search}%")
            )
            .Where(mp => query.IkunkId == null || mp.Ikunk.Id == query.IkunkId)
            .Where(mp => query.FlowDirection == null || mp.FlowDirection == query.FlowDirection)
            .Where(mp => query.Transport == null || mp.Transport == query.Transport)
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponse());

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
                query.IncludeLinks ? mp => CreateLinksForMeasuringPoint(mp.Id, query.Fields) : null)
        };
        if (query.IncludeLinks)
        {
            response.Links = CreateLinksForMeasuringPoints(query, response.HasNextPage, response.HasPreviousPage);
        }

        return Ok(response);
    }

    [HttpGet]
    [ApiVersion(2.0)]
    public async Task<IActionResult> GetMeasuringPointsV2(
        [FromQuery] MeasuringPointsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointResponse, MeasurementPoint>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<MeasurementPointResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointResponse, MeasurementPoint>();

        IQueryable<MeasurementPointResponseV2> measurementPointQuery = dbContext
            .MeasurementPoints
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.Name, $"%{query.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{query.Search}%")
            )
            .Where(mp => query.IkunkId == null || mp.Ikunk.Id == query.IkunkId)
            .Where(mp => query.FlowDirection == null || mp.FlowDirection == query.FlowDirection)
            .Where(mp => query.Transport == null || mp.Transport == query.Transport)
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponseV2());

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
                query.IncludeLinks ? mp => CreateLinksForMeasuringPoint(mp.Id, query.Fields) : null)
        };
        if (query.IncludeLinks)
        {
            response.Links = CreateLinksForMeasuringPoints(query, response.HasNextPage, response.HasPreviousPage);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeasuringPoint(
        string id,
        [FromQuery] SingleQueryParameters query,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<MeasurementPointResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        MeasurementPointResponse? measuringPointResponse = await dbContext
            .MeasurementPoints
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponse())
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (measuringPointResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedResponse = dataShapingService.ShapeData(measuringPointResponse, query.Fields);
        if (query.IncludeLinks)
        {
            shapedResponse.TryAdd("links", CreateLinksForMeasuringPoint(id, query.Fields));
        }

        return Ok(shapedResponse);
    }

    [HttpPost]
    public async Task<ActionResult<MeasurementPointResponse>> CreateMeasuringPoint(
        [FromBody] CreateMeasuringPointRequest request,
        IValidator<CreateMeasuringPointRequest> validator,
        CancellationToken cancellationToken
        )
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await dbContext.Ikunks.AnyAsync(i => i.Id == request.IkunkId, cancellationToken))
        {
            return Problem(
                detail: "Ikunk with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (await dbContext.MeasurementPoints.AnyAsync(mp => mp.Name == request.Name || mp.ControlPoint == request.ControlPoint, cancellationToken))
        {
            return Problem(
                detail: "Measurement point with specific name or control point already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        MeasurementPoint measuringPoint = request.ToEntity(user.Email);
        await dbContext.MeasurementPoints.AddAsync(measuringPoint, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        MeasurementPointResponse measurementPointResponse = measuringPoint.ToResponse() with
        {
            Links = CreateLinksForMeasuringPoint(measuringPoint.Id)
        };

        return CreatedAtAction(
            actionName: nameof(GetMeasuringPoint),
            routeValues: new { id = measurementPointResponse.Id },
            value: measurementPointResponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMeasurementPoint(
        string id,
        [FromBody] UpdateMeasurementPointRequest request,
        IValidator<UpdateMeasurementPointRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (request.IkunkId is not null && !await dbContext.Ikunks.AnyAsync(i => i.Id == request.IkunkId, cancellationToken))
        {
            return Problem(
                detail: "Ikunk with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        MeasurementPoint? measuringPoint = await dbContext.MeasurementPoints.FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (measuringPoint is null)
        {
            return Problem(
                detail: "Measurement point with specific id does not exists!",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (await dbContext.MeasurementPoints.AnyAsync(mp => (mp.Name == request.Name || mp.ControlPoint == request.ControlPoint) && mp.Id != id, cancellationToken))
        {
            return Problem(
                detail: "Measurement point with specific name or control point already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        measuringPoint.UpdateFromRequest(request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult> DeactivateMeasurementPoint(
        string id,
        [FromBody] DateOnly activeTo,
        CancellationToken cancellationToken)
    {
        MeasurementPoint? measuringPoint = await dbContext.MeasurementPoints.FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (measuringPoint is null)
        {
            return Problem(
                detail: "Measurement point with specific id does not exists!",
                statusCode: StatusCodes.Status404NotFound);
        }

        measuringPoint.ActiveTo = activeTo;
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("transports")]
    public ActionResult<List<EnumTypeResponse>> GetTransports()
    {
        List<EnumTypeResponse> transports = [.. Enum
            .GetValues<TransportType>()
            .Select(t => new EnumTypeResponse
            {
                Value = t,
                Description = t.GetDescription()
            })];
        return Ok(transports);
    }

    [HttpGet("flow-directions")]
    public ActionResult<List<EnumTypeResponse>> GetFlowDirections()
    {
        List<EnumTypeResponse> transports = [.. Enum
            .GetValues<FlowDirectionType>()
            .Select(t => new EnumTypeResponse
            {
                Value = t,
                Description = t.GetDescription()
            })];
        return Ok(transports);
    }

    private List<LinkResponse> CreateLinksForMeasuringPoints(
        MeasuringPointsQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkResponse> links = 
        [
            linkService.Create(nameof(GetMeasuringPoints), "self", HttpMethods.Get, new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }),
            linkService.Create(nameof(CreateMeasuringPoint), "create", HttpMethods.Post),
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetMeasuringPoints), "next-page", HttpMethods.Get, new
            {
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields,
                q = parameters.Search,
                sort = parameters.Sort,
                ikunkid = parameters.IkunkId,
                direction = parameters.FlowDirection,
                transport = parameters.Transport
            }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(nameof(GetMeasuringPoints), "previous-page", HttpMethods.Get, new
                {
                    page = parameters.Page - 1,
                    pageSize = parameters.PageSize,
                    fields = parameters.Fields,
                    q = parameters.Search,
                    sort = parameters.Sort,
                    ikunkid = parameters.IkunkId,
                    direction = parameters.FlowDirection,
                    transport = parameters.Transport
                })
            );
        }

        return links;
    }

    private List<LinkResponse> CreateLinksForMeasuringPoint(string id, string? fields = null)
    {
        List<LinkResponse> links =
        [
            linkService.Create(nameof(GetMeasuringPoint), "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(nameof(UpdateMeasurementPoint), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(DeactivateMeasurementPoint), "deactivate", HttpMethods.Put, new { id })
        ];

        return links;
    }


}
