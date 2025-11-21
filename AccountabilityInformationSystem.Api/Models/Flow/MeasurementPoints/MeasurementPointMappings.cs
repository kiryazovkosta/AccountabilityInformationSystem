using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Extensions;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public static class MeasurementPointMappings
{
    public static readonly SortMappingDefinition<MeasurementPointResponse, MeasurementPoint> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(MeasurementPointResponse.Name), nameof(MeasurementPoint.Name)),
            new SortMapping(nameof(MeasurementPointResponse.FullName), nameof(MeasurementPoint.FullName)),
            new SortMapping(nameof(MeasurementPointResponse.Description), nameof(MeasurementPoint.Description)),
            new SortMapping(nameof(MeasurementPointResponse.ControlPoint), nameof(MeasurementPoint.ControlPoint)),
            new SortMapping(nameof(MeasurementPointResponse.OrderPosition), nameof(MeasurementPoint.OrderPosition)),
            new SortMapping(nameof(MeasurementPointResponse.FlowDirection), nameof(MeasurementPoint.FlowDirection)),
            new SortMapping(nameof(MeasurementPointResponse.Transport), nameof(MeasurementPoint.Transport)),
            new SortMapping(nameof(MeasurementPointResponse.ActiveFrom), nameof(MeasurementPoint.ActiveFrom)),
            new SortMapping(nameof(MeasurementPointResponse.ActiveTo), nameof(MeasurementPoint.ActiveTo)),
            new SortMapping(
                $"{nameof(MeasurementPointResponse.Ikunk)}.{nameof(MeasurementPointResponse.Ikunk.Id)}", 
                $"{nameof(MeasurementPoint.Ikunk)}.{nameof(MeasurementPoint.Ikunk.Id)}")
        ]
    };


    public static MeasurementPoint ToEntity(this CreateMeasuringPointRequest request, string userName)
        => new()
        {
            Id = $"mp_{Guid.CreateVersion7()}",
            Name = request.Name,
            FullName = request.FullName,
            Description = request.Description,
            ControlPoint = request.ControlPoint,
            OrderPosition = request.OrderPosition,
            FlowDirection = request.FlowDirection,
            Transport = request.Transport,
            ActiveFrom = request.ActiveFrom,
            ActiveTo = request.ActiveTo,
            IkunkId = request.IkunkId,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow,
        };

    public static MeasurementPointResponse ToResponse(this MeasurementPoint measurementPoint)
        => new()
        {
            Id = measurementPoint.Id,
            Name = measurementPoint.Name,
            FullName = measurementPoint.FullName,
            Description = measurementPoint.Description,
            ControlPoint = measurementPoint.ControlPoint,
            OrderPosition = measurementPoint.OrderPosition,
            FlowDirection = new() 
            { 
                Value = measurementPoint.FlowDirection, 
                Description = measurementPoint.FlowDirection.GetDescription() 
            },
            Transport = new()
            {
                Value = measurementPoint.Transport,
                Description = measurementPoint.Transport.GetDescription()
            },
            ActiveFrom = measurementPoint.ActiveFrom,
            ActiveTo = measurementPoint.ActiveTo,
            Ikunk = measurementPoint?.Ikunk is not null ?
                new()
                {
                    Id = measurementPoint.Ikunk.Id,
                    Name = measurementPoint.Ikunk.Name,
                    Warehouse = new()
                    {
                        Id = measurementPoint.Ikunk.Warehouse.Id,
                        Name = measurementPoint.Ikunk.Warehouse.Name
                    }
                } : null
        };

    public static void UpdateFromRequest(
        this MeasurementPoint measuringPoint, 
        UpdateMeasurementPointRequest request,
        string userName)
    {
        measuringPoint.Name = request.Name ?? measuringPoint.Name;
        measuringPoint.FullName = request.FullName ?? measuringPoint.FullName;
        measuringPoint.Description = request.Description ?? measuringPoint.Description;
        measuringPoint.OrderPosition = request.OrderPosition ?? measuringPoint.OrderPosition;
        measuringPoint.FlowDirection = request.FlowDirection ?? measuringPoint.FlowDirection;
        measuringPoint.Transport = request.Transport ?? measuringPoint.Transport;
        measuringPoint.ControlPoint = request.ControlPoint ?? measuringPoint.ControlPoint;
        measuringPoint.ActiveFrom = request.ActiveFrom ?? measuringPoint.ActiveFrom;
        measuringPoint.ActiveTo = request.ActiveTo ?? measuringPoint.ActiveTo;
        measuringPoint.IkunkId = request.IkunkId ?? measuringPoint.IkunkId;
        measuringPoint.ModifiedBy = userName;
        measuringPoint.ModifiedAt = DateTime.UtcNow;
    }
}
