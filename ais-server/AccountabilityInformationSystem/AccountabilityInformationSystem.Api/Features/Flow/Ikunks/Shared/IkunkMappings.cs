using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;

internal static class IkunkMappings
{
    public static readonly SortMappingDefinition<IkunkResponse, Ikunk> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(IkunkResponse.Name), nameof(Ikunk.Name)),
            new SortMapping(nameof(IkunkResponse.FullName), nameof(Ikunk.FullName)),
            new SortMapping(nameof(IkunkResponse.Description), nameof(Ikunk.Description)),
            new SortMapping(nameof(IkunkResponse.OrderPosition), nameof(Ikunk.OrderPosition)),
            new SortMapping(nameof(IkunkResponse.ActiveFrom), nameof(Ikunk.ActiveFrom)),
            new SortMapping(nameof(IkunkResponse.ActiveTo), nameof(Ikunk.ActiveTo)),
            new SortMapping(
                $"{nameof(IkunkResponse.Warehouse)}.{nameof(IkunkResponse.Warehouse.Id)}",
                $"{nameof(Ikunk.Warehouse)}.{nameof(Ikunk.Warehouse.Id)}")
        ]
    };
}
