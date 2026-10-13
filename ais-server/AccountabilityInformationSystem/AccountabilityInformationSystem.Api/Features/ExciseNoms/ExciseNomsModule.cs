using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms;

internal sealed class ExciseNomsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<ExciseNomenclatureResponse, ApCode>>(_ =>
            ExciseNomenclatureMappings.SortMappingApCode);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<ExciseNomenclatureResponse, BrandName>>(_ =>
            ExciseNomenclatureMappings.SortMappingBrandName);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<ExciseNomenclatureResponse, CnCode>>(_ =>
            ExciseNomenclatureMappings.SortMappingCnCode);
    }
}
