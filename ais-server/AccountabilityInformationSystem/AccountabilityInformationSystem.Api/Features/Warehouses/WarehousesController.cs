using System.Dynamic;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.CreateWarehouse;
using AccountabilityInformationSystem.Api.Features.Warehouses.UpdateWarehouse;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public sealed class WarehousesController(
    ApplicationDbContext dbContext,
    UserContext userContext) : ControllerBase
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
    public async Task<ActionResult<WarehouseResponse>> CreateWarehouse(
        [FromBody] CreateWarehouseRequest request,
        IValidator<CreateWarehouseRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        if (await dbContext.Warehouses.AnyAsync(w => w.ExciseNumber == request.ExciseNumber, cancellationToken))
        {
            return Problem(
                detail: "Warehouse with the same name or excise number already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        Warehouse warehouse = request.ToEntity(user.Email);
        await dbContext.Warehouses.AddAsync(warehouse, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        WarehouseResponse warehouseResponse = warehouse.ToResponse();
        return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id }, warehouseResponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateWarehouse(
        string id,
        [FromBody] UpdateWarehouseRequest request,
        IValidator<UpdateWarehouseRequest> validator,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        bool exciseNumberExists = await dbContext
            .Warehouses
            .AnyAsync(w => w.ExciseNumber == request.ExciseNumber && w.Id != id, cancellationToken);
        if (exciseNumberExists)
        {
            return Problem(
                detail: "Warehouse with the same excise number already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        Warehouse? warehouse = await dbContext
            .Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
        if (warehouse is null)
        {
            return Problem(
                detail: "Warehouse with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        warehouse.UpdateFromRequest(request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWarehouse(string id, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await dbContext
            .Warehouses
            .Include(warehouse => warehouse.Ikunks)
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
        if (warehouse is null)
        {
            return Problem(
                detail: "Warehouse with specific id does not exists!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (warehouse.Ikunks.Count > 0)
        {
            return Problem(
                detail: "Cannot delete warehouse with associated ikunks!",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
