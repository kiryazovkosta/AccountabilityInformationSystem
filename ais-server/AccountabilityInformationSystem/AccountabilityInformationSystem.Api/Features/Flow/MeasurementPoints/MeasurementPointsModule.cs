using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints;

internal sealed class MeasurementPointsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<MeasurementPointResponse, MeasurementPoint>>(_ =>
            MeasurementPointMappings.SortMapping);
        services.AddTransient<MeasuringPointLinkService>();
    }
}
