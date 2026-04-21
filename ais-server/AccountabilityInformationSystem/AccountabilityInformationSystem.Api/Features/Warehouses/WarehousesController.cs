using System.Dynamic;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.Delete;
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
using AccountabilityInformationSystem.Api.Features.Warehouses.Create;

namespace AccountabilityInformationSystem.Api.Features.Warehouses;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public sealed class WarehousesController(
    ApplicationDbContext dbContext,
    IMessageBus bus) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetWarehouses(
        [FromQuery] QueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<WarehouseResponse, Warehouse>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid sort parameter. {query.Sort}");
        }

        if (!dataShapingService.Validate<WarehouseResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {query.Fields}");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<WarehouseResponse, Warehouse>();

        IQueryable<WarehouseResponse> warehousesQuery = dbContext
            .Warehouses
            .Where(mp =>
                query.Search == null ||
                EF.Functions.Like(mp.Name, $"%{query.Search}%") ||
                EF.Functions.Like(mp.FullName, $"%{query.Search}%") ||
                EF.Functions.Like(mp.ExciseNumber, $"%{query.Search}%") ||
                mp.Description != null && EF.Functions.Like(mp.Description, $"%{query.Search}%")
            )
            .ApplySort(query.Sort, sortMappings)
            .AsNoTracking()
            .Select(WarehouseQueries.ProjectToResponse());

        PaginationResponse<ExpandoObject> response = new()
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = await warehousesQuery.CountAsync(cancellationToken),
            Items = dataShapingService.ShapeCollectionData(
                await warehousesQuery
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken),
                query.Fields)
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouseById(
        string id,
        string? fields,
        DataShapingService dataShapingService,
        CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<WarehouseResponse>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid fields parameter. {fields}");
        }

        WarehouseResponse? warehouseResponse = await dbContext
            .Warehouses
            .AsNoTracking()
            .Select(WarehouseQueries.ProjectToResponse())
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
        if (warehouseResponse is null)
        {
            return NotFound();
        }

        ExpandoObject shapedData = dataShapingService.ShapeData(warehouseResponse, fields);
        return Ok(shapedData);
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
