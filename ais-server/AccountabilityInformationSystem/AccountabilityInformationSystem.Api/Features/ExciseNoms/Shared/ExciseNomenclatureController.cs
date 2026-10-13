using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.GetAll;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.GetById;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;

[ResponseCache(Duration = 120, VaryByQueryKeys = ["*"])]
[Authorize]
[ApiController]
[ApiVersion("1.0")]
public abstract class ExciseNomenclatureController<TEntity, TCreateRequest, TUpdateRequest> : ApiController
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
    where TUpdateRequest : UpdateExciseNomenclatureRequest
{
    // Each concrete controller merges the route id into its own request type, since `with` needs the concrete record.
    protected abstract TUpdateRequest WithId(TUpdateRequest request, string id);

    [HttpGet]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ExciseNomenclatureQueryParameters query,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> result = await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(
            new GetAllExciseNomenclaturesRequest<TEntity>(query), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> GetById(
        string id,
        [FromQuery] FieldsOnlyQueryParameters query,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        Result<ExpandoObject> result = await bus.InvokeAsync<Result<ExpandoObject>>(
            new GetExciseNomenclatureByIdRequest<TEntity>(id, query), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> Create(
        [FromBody] TCreateRequest request,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        Result<ExciseNomenclatureResponse> result = await bus.InvokeAsync<Result<ExciseNomenclatureResponse>>(
            request,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = result.Value!.Id },
            value: result.Value);
    }

    [HttpPost("batch")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> CreateBatch(
        [FromBody] CreateExciseNomenclatureBatchRequest<TCreateRequest> request,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] TUpdateRequest request,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        request = WithId(request, id);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{id}/toggle-status")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> ToggleStatus(
        string id,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(
            new ToggleExciseNomenclatureStatusRequest<TEntity>(id), cancellationToken);
        return result.ToActionResult();
    }
}
