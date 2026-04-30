using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Create;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetAll;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetById;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData;

[ApiController]
[Route("api/flow/measuring-points-data")]
[ApiVersion(1.0)]
[Authorize(Roles = $"{Role.Admin},{Role.FlowUser}")]
public class MeasurementPointsDataController(
    IMessageBus bus) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces(typeof(PaginationResponse<MeasurementPointDataListResponse>))]
    public async Task<IActionResult> GetMeasuringPointsData(
        [FromQuery] GetMeasurementPointsDataRequest request,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result 
            = await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetMeasuringPointData(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        CancellationToken cancellationToken)
    {
        GetMeasuringPointDataRequest request = new(id, query.IncludeLinks, query.Fields);
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> CreateMeasuringPointData(
        [FromBody] CreateMeasuringPointDataRequest request,
        IValidator<CreateMeasuringPointDataRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<MeasurementPointDataResponse> result = await bus.InvokeAsync<Result<MeasurementPointDataResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(
            actionName: nameof(GetMeasuringPointData),
            routeValues: new { id = result.Value!.Id },
            value: result.Value);
    }
}
