using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Warehouses;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public static class IkunkQueries
{
    public static Expression<Func<Ikunk, IkunkResponse>> ProjectToResponse()
    {
        return Ikunk => new IkunkResponse
        {
            Id = Ikunk.Id,
            Name = Ikunk.Name,
            FullName = Ikunk.FullName,
            Description = Ikunk.Description,
            OrderPosition = Ikunk.OrderPosition,
            ActiveFrom = Ikunk.ActiveFrom,
            ActiveTo = Ikunk.ActiveTo,
            Warehouse = new WarehouseListResponse
            {
                Id = Ikunk.Warehouse.Id,
                FullName = Ikunk.Warehouse.FullName
            },
            MeasurementPoints = Ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new MeasurementPointListResponse
                {
                    Id = mp.Id,
                    FullName = mp.FullName,
                })
                .ToList()
        };
    }
}
