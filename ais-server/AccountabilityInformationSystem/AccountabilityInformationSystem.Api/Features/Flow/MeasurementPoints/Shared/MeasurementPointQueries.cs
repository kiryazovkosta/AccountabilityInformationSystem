using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

internal static class MeasurementPointQueries
{
    public static Expression<Func<MeasurementPoint, MeasurementPointResponse>> ProjectToResponse()
    {
        return mp => new MeasurementPointResponse
        {
            Id = mp.Id,
            Name = mp.Name,
            FullName = mp.FullName,
            Description = mp.Description,
            ControlPoint = mp.ControlPoint,
            OrderPosition = mp.OrderPosition,
            FlowDirection = new EnumTypeResponse
            {
                Value = mp.FlowDirection,
                Description = mp.FlowDirection.GetDescription()
            },
            Transport = new EnumTypeResponse
            {
                Value = mp.Transport,
                Description = mp.Transport.GetDescription()
            },
            ActiveFrom = mp.ActiveFrom,
            ActiveTo = mp.ActiveTo,
            Ikunk = new MeasurementPointIkunkResponse
            {
                Id = mp.Ikunk.Id,
                Name = mp.Ikunk.Name,
                Warehouse = new MeasurementPointIkunkWarehouseResponse
                {
                    Id = mp.Ikunk.WarehouseId,
                    Name = mp.Ikunk.Warehouse.Name
                }
            }
        };
    }

    public static Expression<Func<MeasurementPoint, MeasurementPointResponseV2>> ProjectToResponseV2()
    {
        return mp => new MeasurementPointResponseV2
        {
            Id = mp.Id,
            FullName = mp.FullName,
            OrderPosition = mp.OrderPosition,
            Ikunk = new MeasurementPointIkunkResponse
            {
                Id = mp.Ikunk.Id,
                Name = mp.Ikunk.Name
            }
        };
    }
}
