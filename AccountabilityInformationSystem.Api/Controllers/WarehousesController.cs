using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Warehouses;
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
        CancellationToken cancellationToken)
    {
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
        CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await dbContext
            .Warehouses
            .FirstOrDefaultAsync(warehouse => warehouse.Id == id, cancellationToken);
        if (warehouse is null)
        {
            return NotFound();
        }

        warehouse.UpdateFromRequest(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
