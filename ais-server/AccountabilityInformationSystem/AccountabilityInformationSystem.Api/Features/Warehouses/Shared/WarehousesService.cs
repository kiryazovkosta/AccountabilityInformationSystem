using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Warehouses.CreateWarehouse;
using AccountabilityInformationSystem.Api.Features.Warehouses.Shared;
using AccountabilityInformationSystem.Api.Features.Warehouses.UpdateWarehouse;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Warehouses;

public sealed class WarehousesService(
    ApplicationDbContext db,
    UserContext userContext)
{
    public async Task<bool> ExistsAsync(string id, CancellationToken ct)
        => await db.Warehouses.AnyAsync(w => w.Id == id, ct);

    public async Task<WarehouseResponse> CreateAsync(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }  

        bool exciseExists = await db.Warehouses
            .AnyAsync(w => w.ExciseNumber == request.ExciseNumber, cancellationToken);
        if (exciseExists)
        {
            throw new InvalidOperationException("Warehouse with the same excise number already exists!");
        }
            

        Warehouse warehouse = request.ToEntity(user.Email);
        await db.Warehouses.AddAsync(warehouse, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return warehouse.ToResponse();
    }

    public async Task UpdateAsync(
        string id,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
            

        bool exciseExists = await db.Warehouses
            .AnyAsync(w => w.ExciseNumber == request.ExciseNumber && w.Id != id, cancellationToken);
        if (exciseExists)
        {
            throw new InvalidOperationException("Warehouse with the same excise number already exists!");
        }
            
        Warehouse? warehouse = await db.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null)
        {
            throw new KeyNotFoundException("Warehouse with specific id does not exist!");
        }
            
        warehouse.UpdateFromRequest(request, user.Email);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await db.Warehouses
            .Include(w => w.Ikunks)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null)
        {
            throw new KeyNotFoundException("Warehouse with specific id does not exist!");
        }
            
        if (warehouse.Ikunks.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete warehouse with associated ikunks!");
        }  

        db.Warehouses.Remove(warehouse);
        await db.SaveChangesAsync(cancellationToken);
    }
}
