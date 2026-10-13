using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords;

internal sealed class WarrantyRecordsModule : IFeatureModule
{
    public void Register(IServiceCollection services) =>
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<WarrantyRecordListResponse, WarrantyRecord>>(_ =>
            WarrantyRecordMappings.SortMapping);
}
