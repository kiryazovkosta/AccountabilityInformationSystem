using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

internal static class IkunkQueries
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
            Warehouse = new IkunkWarehouseResponse
            {
                Id = ikunk.Warehouse.Id,
                FullName = ikunk.Warehouse.FullName
            },
            MeasurementPoints = ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new IkunkMeasurementPointResponse
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
            Warehouse = new IkunkWarehouseResponse
            {
                Id = ikunk.Warehouse.Id,
                FullName = ikunk.Warehouse.FullName
            },
            MeasurementPoints = ikunk.MeasurementPoints
                .OrderBy(mp => mp.OrderPosition)
                .Select(mp => new IkunkMeasurementPointResponse
                {
                    Id = mp.Id,
                    FullName = mp.FullName,
                    ControlPoint = mp.ControlPoint
                })
                .ToList()
        };
    }
}
