using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Warehouses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehousesController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WarehousesCollectionResponse>> GetWarehouses(CancellationToken cancellationToken)
    {
        List<WarehouseResponse> warehousesResponse = await dbContext
            .Warehouses
            .Include(warehouse => warehouse.Ikunks)
            .ThenInclude(ikunk => ikunk.MeasurementPoints)
            .AsNoTracking()
            .OrderBy(warehouse => warehouse.OrderPosition)
            .Select(WarehouseQueries.ProjectToResponse())
            .ToListAsync(cancellationToken);
        return Ok(new WarehousesCollectionResponse() { Items = warehousesResponse });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseResponse>> GetWarehouseById(string id, CancellationToken cancellationToken)
    {
        WarehouseResponse? warehouseResponse = await dbContext
            .Warehouses
            .AsNoTracking()
            .Include(warehouse => warehouse.Ikunks.Where(ikunk => !ikunk.IsDeleted))
            .ThenInclude(ikunk => ikunk.MeasurementPoints.Where(mp => !mp.IsDeleted))
            .Select(WarehouseQueries.ProjectToResponse())
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
        if (warehouseResponse is null)
        {
            return NotFound();
        }

        return Ok(warehouseResponse);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseResponse>> CreateWarehouse(
        [FromBody] CreateWarehouseRequest request,
        IValidator<CreateWarehouseRequest> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (await dbContext.Warehouses.AnyAsync(w => w.ExciseNumber == request.ExciseNumber, cancellationToken))
        {
            return Problem(
                detail: "Warehouse with the same name or excise number already exists!",
                statusCode: StatusCodes.Status409Conflict);
        }

        Warehouse warehouse = request.ToEntity();
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

        warehouse.UpdateFromRequest(request);
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

        warehouse.IsDeleted = true;
        warehouse.DeletedAt = DateTime.UtcNow;
        warehouse.DeletedBy = "System user"; // TODO: Replace with actual user
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
