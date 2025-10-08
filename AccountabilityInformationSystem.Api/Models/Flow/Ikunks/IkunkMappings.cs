using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Warehouses;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public static class IkunkMappings
{
    public static Ikunk ToEntity(this CreateIkunkRequest request)
        => new()
    {
        Id = $"ik_{Guid.CreateVersion7()}",
        Name = request.Name,
        FullName = request.FullName,
        Description = request.Description,
        OrderPosition = request.OrderPosition,
        ActiveFrom = request.ActiveFrom,
        ActiveTo = request.ActiveTo,
        WarehouseId = request.WarehouseId,
        // TODO: Replace with actual user
        CreatedBy = "System user",
        CreatedAt = DateTime.UtcNow,
        IsDeleted = false
    };

    public static IkunkResponse ToResponse(this Ikunk ikunk)
        => new()
        {
            Id = ikunk.Id,
            Name = ikunk.Name,
            FullName = ikunk.FullName,
            Description = ikunk.Description,
            OrderPosition = ikunk.OrderPosition,
            ActiveFrom = ikunk.ActiveFrom,
            ActiveTo = ikunk.ActiveTo,
            Warehouse = ikunk.Warehouse is not null ? 
                new WarehouseListResponse() 
                { 
                    Id = ikunk.Warehouse.Id, 
                    FullName = ikunk.Warehouse.FullName 
                } : null ,
            MeasurementPoints = [.. ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new MeasurementPointListResponse()
                {
                    Id = mp.Id,
                    FullName = mp.FullName
                })]
        };

    public static void UpdateFromRequest(this Ikunk ikunk, UpdateIkunkRequest request)
    {
        ikunk.Name = request.Name ?? ikunk.Name;
        ikunk.FullName = request.FullName ?? ikunk.FullName;
        ikunk.Description = request.Description ?? ikunk.Description;
        ikunk.OrderPosition = request.OrderPosition ?? ikunk.OrderPosition;
        ikunk.ActiveFrom = request.ActiveFrom ?? ikunk.ActiveFrom;
        ikunk.ActiveTo = request.ActiveTo ?? ikunk.ActiveTo;
        ikunk.WarehouseId = request.WarehouseId ?? ikunk.WarehouseId;
        // TODO: Replace with actual user
        ikunk.ModifiedBy = "System User";
        ikunk.ModifiedAt = DateTime.UtcNow;
    }
}
