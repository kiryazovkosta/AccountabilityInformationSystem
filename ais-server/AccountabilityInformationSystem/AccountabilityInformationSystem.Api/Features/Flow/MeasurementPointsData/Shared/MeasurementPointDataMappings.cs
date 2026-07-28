using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

internal static class MeasurementPointDataMappings
{
    public static readonly SortMappingDefinition<MeasurementPointDataListResponse, MeasurementPointData> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(MeasurementPointDataListResponse.Number), nameof(MeasurementPointData.Number)),
            new SortMapping(nameof(MeasurementPointDataListResponse.BeginTime), nameof(MeasurementPointData.BeginTime)),
            new SortMapping(nameof(MeasurementPointDataListResponse.EndTime), nameof(MeasurementPointData.EndTime)),
            new SortMapping(nameof(MeasurementPointDataListResponse.FlowDirection), nameof(MeasurementPointData.FlowDirectionType)),
            new SortMapping(nameof(MeasurementPointDataListResponse.GrossObservableVolume), nameof(MeasurementPointData.GrossObservableVolume)),
            new SortMapping(nameof(MeasurementPointDataListResponse.GrossStandardVolume), nameof(MeasurementPointData.GrossStandardVolume)),
            new SortMapping(nameof(MeasurementPointDataListResponse.Mass), nameof(MeasurementPointData.Mass)),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.MeasurementPoint)}.{nameof(MeasurementPointDataListResponse.MeasurementPoint.Id)}",
                $"{nameof(MeasurementPointData.MeasurementPoint)}.{nameof(MeasurementPointData.MeasurementPoint.Id)}"),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.MeasurementPoint)}.{nameof(MeasurementPointDataListResponse.MeasurementPoint.ControlPoint)}",
                $"{nameof(MeasurementPointData.MeasurementPoint)}.{nameof(MeasurementPointData.MeasurementPoint.ControlPoint)}"),
            new SortMapping(
                $"{nameof(MeasurementPointDataListResponse.Product)}.{nameof(MeasurementPointDataListResponse.Product.Code)}",
                $"{nameof(MeasurementPointData.Product)}.{nameof(MeasurementPointData.Product.Code)}")
        ]
    };
}
