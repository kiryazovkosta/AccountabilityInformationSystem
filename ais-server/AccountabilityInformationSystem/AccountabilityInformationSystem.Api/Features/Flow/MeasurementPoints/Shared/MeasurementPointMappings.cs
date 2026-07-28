using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

internal static class MeasurementPointMappings
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
}
