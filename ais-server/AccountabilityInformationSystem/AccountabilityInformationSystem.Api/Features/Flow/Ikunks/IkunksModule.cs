using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks;

internal sealed class IkunksModule : IFeatureModule
{
    public void Register(IServiceCollection services) =>
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<IkunkResponse, Ikunk>>(_ =>
            IkunkMappings.SortMapping);
}
