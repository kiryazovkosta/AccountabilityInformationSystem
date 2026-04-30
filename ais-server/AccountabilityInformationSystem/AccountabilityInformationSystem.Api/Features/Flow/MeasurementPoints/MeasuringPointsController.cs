using System.Dynamic;
using System.Net.Mime;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Create;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetById;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints;

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
    IMessageBus bus) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<MeasurementPointResponse>))]
    public async Task<IActionResult> GetMeasuringPoints(
        [FromQuery] GetMeasuringPointsRequest request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result 
            = await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    [ApiVersion(2.0)]
    public async Task<IActionResult> GetMeasuringPointsV2(
        [FromQuery] GetMeasuringPointsV2Request request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result
            = await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeasuringPoint(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        CancellationToken cancellationToken)
    {
        GetGetMeasuringPointRequest request = new(id, query.IncludeLinks, query.Fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeasuringPoint(
        [FromBody] CreateMeasuringPointRequest request,
        IValidator<CreateMeasuringPointRequest> validator,
        CancellationToken cancellationToken
        )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<MeasurementPointResponse> result = await bus.InvokeAsync<Result<MeasurementPointResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetMeasuringPoint), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeasurementPoint(
        string id,
        [FromBody] UpdateMeasurementPointRequest request,
        IValidator<UpdateMeasurementPointRequest> validator,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateMeasurementPoint(
        string id,
        [FromBody] DateOnly activeTo,
        CancellationToken cancellationToken)
    {
        DeactivateMeasuringPointRequest request = new(id, activeTo);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
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
