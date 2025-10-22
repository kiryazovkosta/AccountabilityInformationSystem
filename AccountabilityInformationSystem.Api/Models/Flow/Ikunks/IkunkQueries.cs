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
        return ikunk => new IkunkResponse
        {
            Id = ikunk.Id,
            Name = ikunk.Name,
            FullName = ikunk.FullName,
            Description = ikunk.Description,
            OrderPosition = ikunk.OrderPosition,
            ActiveFrom = ikunk.ActiveFrom,
            ActiveTo = ikunk.ActiveTo,
            Warehouse = new WarehouseListResponse
            {
                Id = ikunk.Warehouse.Id,
                FullName = ikunk.Warehouse.FullName
            },
            MeasurementPoints = ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new MeasurementPointListResponse
                {
                    Id = mp.Id,
                    FullName = mp.FullName,
                    ControlPoint = mp.ControlPoint
                })
                .ToList()
        };
    }

    public static Expression<Func<Ikunk, IkunkResponseV2>> ProjectToResponseV2()
    {
        return ikunk => new IkunkResponseV2
        {
            Id = ikunk.Id,
            Name = ikunk.Name,
            FullName = ikunk.FullName,
            Description = ikunk.Description,
            OrderPosition = ikunk.OrderPosition,
            ActiveFrom = ikunk.ActiveFrom,
            ActiveTo = ikunk.ActiveTo,
            Created = ikunk.CreatedAt,
            Modified = ikunk.ModifiedAt,
            Warehouse = new WarehouseListResponse
            {
                Id = ikunk.Warehouse.Id,
                FullName = ikunk.Warehouse.FullName
            },
            MeasurementPoints = ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new MeasurementPointListResponse
                {
                    Id = mp.Id,
                    FullName = mp.FullName,
                    ControlPoint = mp.ControlPoint
                })
                .ToList()
        };
    }
}
