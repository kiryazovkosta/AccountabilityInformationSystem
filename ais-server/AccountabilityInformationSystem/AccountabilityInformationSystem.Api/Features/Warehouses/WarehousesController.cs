using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;
using AccountabilityInformationSystem.Api.Features.Warehouses.Delete;
using AccountabilityInformationSystem.Api.Features.Warehouses.GetAll;
using AccountabilityInformationSystem.Api.Features.Warehouses.GetById;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Warehouses;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public sealed class WarehousesController(IMessageBus bus) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetWarehouses(
        [FromQuery] GetWarehousesRequest query,
        CancellationToken cancellationToken)
    {
        Result<PaginationResponse<ExpandoObject>> response = 
            await bus.InvokeAsync<Result<PaginationResponse<ExpandoObject>>>(query, cancellationToken);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouseById(
        string id,
        string? fields,
        CancellationToken cancellationToken)
    {
        Result<ExpandoObject> response = 
            await bus.InvokeAsync<Result<ExpandoObject>>(new GetWarehouseByIdRequest(id, fields), cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(
        [FromBody] CreateWarehouseRequest request,
        IValidator<CreateWarehouseRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result<WarehouseResponse> result = await bus.InvokeAsync<Result<WarehouseResponse>>(request, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetWarehouseById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWarehouse(
        string id,
        [FromBody] UpdateWarehouseRequest request,
        IValidator<UpdateWarehouseRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        Result result = await bus.InvokeAsync<Result>(new UpdateWarehouseCommand(id, request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarehouse(string id, CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(new DeleteWarehouseRequest(id), cancellationToken);
        return result.ToActionResult();
    }
}
