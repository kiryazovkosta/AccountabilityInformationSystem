using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

internal static class WarehouseQueries
{
    public static Expression<Func<Warehouse, WarehouseResponse>> ProjectToResponse()
    {
        return warehouse => new WarehouseResponse()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            FullName = warehouse.FullName,
            Description = warehouse.Description,
            OrderPosition = warehouse.OrderPosition,
            ExciseNumber = warehouse.ExciseNumber,
            ActiveFrom = warehouse.ActiveFrom,
            ActiveTo = warehouse.ActiveTo,
            Ikunks = warehouse.Ikunks
                .OrderBy(ikunk => ikunk.OrderPosition)
                .Select(ikunk => new WarehouseIkunkResponse()
                {
                    Id = ikunk.Id,
                    FullName = ikunk.FullName,
                    MeasurementPoints = ikunk.MeasurementPoints
                        .OrderBy(mp => mp.OrderPosition)
                        .Select(mp => new WarehouseIkunkMeasurementPointResponse()
                        {
                            Id = mp.Id,
                            FullName = mp.FullName,
                            ControlPoint = mp.ControlPoint
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
