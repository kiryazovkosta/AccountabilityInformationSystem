using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetAll;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Wolverine;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;

[ResponseCache(Duration = 120)]
[Authorize]
[ApiController]
[ApiVersion(1.0)]
public abstract class ExciseNomenclatureController<TEntity, TCreateRequest, TUpdateRequest> : ControllerBase
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
    where TUpdateRequest : UpdateExciseNomenclatureRequest
{
    protected abstract string EntityIdPrefix { get; }

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
        [FromServices] IValidator<TCreateRequest> validator,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Result<ExciseNomenclatureResponse> result = await bus.InvokeAsync<Result<ExciseNomenclatureResponse>>(
            new CreateExciseNomenclatureCommand<TEntity, TCreateRequest>(request, EntityIdPrefix),
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
        [FromServices] IValidator<CreateExciseNomenclatureBatchRequest<TCreateRequest>> validator,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Result result = await bus.InvokeAsync<Result>(
            new CreateExciseNomenclatureBatchCommand<TEntity, TCreateRequest>(request.Entries, EntityIdPrefix),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    [MapToApiVersion(1.0)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] TUpdateRequest request,
        [FromServices] IValidator<TUpdateRequest> validator,
        [FromServices] IMessageBus bus,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result result = await bus.InvokeAsync<Result>(
            new UpdateExciseNomenclatureCommand<TEntity>(id, request), cancellationToken);
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
            new ToggleExciseNomenclatureStatusCommand<TEntity>(id), cancellationToken);
        return result.ToActionResult();
    }
}
