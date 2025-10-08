using System.Linq.Dynamic.Core;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Services.Sorting;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers.Flow;

[ApiController]
[Route("api/flow/measuring-points")]
public sealed class MeasuringPointsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<MeasurementPointsCollectionResponse>> GetMeasuringPoints(
        [FromQuery] MeasuringPointsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<MeasurementPointResponse, MeasurementPoint>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}"
            );
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<MeasurementPointResponse, MeasurementPoint>();

        List<MeasurementPointResponse> measuringPointsResponse = await dbContext.MeasurementPoints
            .Include(mp => mp.Ikunk)
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.Name, $"%{query.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{query.Search}%")
            )
            .Where(mp => query.Direction == null || mp.FlowDirection == query.Direction)
            .Where(mp => query.Transport == null || mp.Transport == query.Transport)
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(MeasurementPointQueries.ProjectToResponse())
            .ToListAsync(cancellationToken);
        return Ok(new MeasurementPointsCollectionResponse() { Items = measuringPointsResponse });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementPointResponse>> GetMeasuringPointById(
        string id,
        CancellationToken cancellationToken)
    {
        MeasurementPointResponse? measuringPointResponse = await dbContext
            .MeasurementPoints
            .AsNoTracking()
            .Include(mp => mp.Ikunk)
            .Select(MeasurementPointQueries.ProjectToResponse())
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
        if (measuringPointResponse is null)
        {
            return NotFound();
        }
        return Ok(measuringPointResponse);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMeasuringPoint(
        [FromBody] CreateMeasuringPointRequest request,
        IValidator<CreateMeasuringPointRequest> validator,
        CancellationToken cancellationToken
        )
    {
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

        MeasurementPoint measuringPoint = request.ToEntity();
        await dbContext.MeasurementPoints.AddAsync(measuringPoint, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        MeasurementPointResponse measurementPointResponse = measuringPoint.ToResponse();
        return CreatedAtAction(
            actionName: nameof(GetMeasuringPointById),
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

        measuringPoint.UpdateFromRequest(request);
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
}
