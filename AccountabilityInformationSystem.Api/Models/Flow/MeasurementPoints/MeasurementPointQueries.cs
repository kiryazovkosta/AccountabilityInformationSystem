using System.Linq.Expressions;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public static class MeasurementPointQueries
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
            FlowDirection = new EnumTypeResponse()
            {
                Value = mp.FlowDirection,
                Description = mp.FlowDirection.GetDescription()
            },
            Transport = new EnumTypeResponse()
            {
                Value = mp.Transport,
                Description = mp.Transport.GetDescription()
            },
            ActiveFrom = mp.ActiveFrom,
            ActiveTo = mp.ActiveTo,
            Ikunk = new IkunkSimpleResponse
            {
                Id = mp.Ikunk.Id,
                Name = mp.Ikunk.Name
            }
        };
    }
}
