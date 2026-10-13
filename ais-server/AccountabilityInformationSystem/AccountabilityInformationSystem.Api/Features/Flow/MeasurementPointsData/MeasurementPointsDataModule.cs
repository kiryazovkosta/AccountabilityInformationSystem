using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData;

internal sealed class MeasurementPointsDataModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<MeasurementPointDataListResponse, MeasurementPointData>>(_ =>
            MeasurementPointDataMappings.SortMapping);
        services.AddTransient<MeasuringPointDataLinkService>();
    }
}
