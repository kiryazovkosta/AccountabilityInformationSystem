using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

internal static class IkunkMappings
{
    public static readonly SortMappingDefinition<IkunkResponse, Ikunk> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(IkunkResponse.Name), nameof(MeasurementPoint.Name)),
            new SortMapping(nameof(IkunkResponse.FullName), nameof(MeasurementPoint.FullName)),
            new SortMapping(nameof(IkunkResponse.Description), nameof(MeasurementPoint.Description)),
            new SortMapping(nameof(IkunkResponse.OrderPosition), nameof(MeasurementPoint.OrderPosition)),
            new SortMapping(nameof(IkunkResponse.ActiveFrom), nameof(MeasurementPoint.ActiveFrom)),
            new SortMapping(nameof(IkunkResponse.ActiveTo), nameof(MeasurementPoint.ActiveTo)),
            new SortMapping(
                $"{nameof(IkunkResponse.Warehouse)}.{nameof(IkunkResponse.Warehouse.Id)}",
                $"{nameof(Ikunk.Warehouse)}.{nameof(Ikunk.Warehouse.Id)}")
        ]
    };
}
