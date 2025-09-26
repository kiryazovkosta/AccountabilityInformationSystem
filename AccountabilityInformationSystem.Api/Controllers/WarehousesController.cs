using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Flow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehousesController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWarehouses(CancellationToken cancellationToken)
    {
        List<Warehouse> warehouses = await dbContext.Warehouses.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(warehouses);
    }
}
