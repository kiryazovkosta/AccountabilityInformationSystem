using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

namespace AccountabilityInformationSystem.Api.Models.Warehouses;

internal static class WarehouseMappings
{
    public static Warehouse ToEntity(this CreateWarehouseRequest request)
        => new()
        {
            Id = $"wh_{Guid.CreateVersion7()}",
            Name = request.Name,
            FullName = request.FullName,
            Description = request.Description,
            OrderPosition = request.OrderPosition,
            ExciseNumber = request.ExciseNumber,
            ActiveFrom = request.ActiveFrom,
            ActiveTo = request.ActiveTo,
            // TODO: Replace with actual user
            CreatedBy = "System user",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

    public static WarehouseResponse ToResponse(this Warehouse warehouse)
    {
        return new WarehouseResponse()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            FullName = warehouse.FullName,
            Description = warehouse.Description,
            OrderPosition = warehouse.OrderPosition,
            ExciseNumber = warehouse.ExciseNumber,
            ActiveFrom = warehouse.ActiveFrom,
            ActiveTo = warehouse.ActiveTo,
            Ikunks = [.. warehouse.Ikunks
                .OrderBy(ikunk => ikunk.OrderPosition)
                .Select(ikunk => new IkunkListResponse()
                {
                    Id = ikunk.Id,
                    FullName = ikunk.FullName,
                    MeasurementPoints = [.. ikunk.MeasurementPoints
                        .OrderBy(mp => mp.OrderPosition)
                        .Select(mp => new Models.Flow.MeasurementPoints.MeasurementPointListResponse()
                        {
                            Id = mp.Id,
                            FullName = mp.FullName
                        })]
                })]
        };
    }

    public static void UpdateFromRequest(this Warehouse warehouse, UpdateWarehouseRequest request)
    {
        warehouse.Name = request.Name ?? warehouse.Name;
        warehouse.FullName = request.FullName ?? warehouse.FullName;
        warehouse.Description = request.Description ?? warehouse.Description;
        warehouse.OrderPosition = request.OrderPosition ?? warehouse.OrderPosition;
        warehouse.ExciseNumber = request.ExciseNumber ?? warehouse.ExciseNumber;
        warehouse.ActiveFrom = request.ActiveFrom ?? warehouse.ActiveFrom;
        warehouse.ActiveTo = request.ActiveTo ?? warehouse.ActiveTo;
        // TODO: Replace with actual user
        warehouse.ModifiedBy = "System User";
        warehouse.ModifiedAt = DateTime.UtcNow;
    }
}
